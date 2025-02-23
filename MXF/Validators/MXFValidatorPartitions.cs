#region license
//
// MXF - Myriadbits .NET MXF library. 
// Read MXF Files.
// Copyright (C) 2015 Myriadbits, Jochem Bakker
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// For more information, contact me at: info@myriadbits.com
//
#endregion

using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Myriadbits.MXF.Extensions;

namespace Myriadbits.MXF.Validators
{
    public class MXFValidatorPartitions : MXFValidator
    {
        private readonly ulong runInHeaderOffset = 0;
        public static readonly int MAJOR_VERSION = 1;
        public static readonly int MINOR_VERSION_SMPTE377M_2004 = 2;
        public static readonly int MINOR_VERSION_SMPTE377M_2011 = 3;

        public MXFValidatorPartitions(MXFFile file) : base(file)
        {
            // if there is a RunIn consider it for the partition offsets
            var runIn = File.Descendants().OfType<MXFRunIn>().SingleOrDefault();
            if (runIn != null)
            {
                runInHeaderOffset = (ulong)runIn.TotalLength;
            }
        }

        public bool AnyPartitionsPresent()
        {
            return this.File.GetPartitions().Any();
        }
        public bool IsHeaderPartitionPresent()
        {
            var firstPartition = this.File.GetPartitions()?.First();
            return firstPartition != null && firstPartition.IsHeaderPartition();
        }

        public bool IsFooterPartitionPresent()
        {
            var lastPartition = this.File.GetPartitions()?.Last();
            return lastPartition != null && lastPartition.IsFooterPartition();
        }

        public bool IsHeaderPartitionUnique()
        {
            int count = this.File.GetPartitions().Where(p => p.IsHeaderPartition()).Count();
            return count <= 1;
        }
        public bool IsFooterPartitionUnique()
        {
            int count = this.File.GetPartitions().Where(p => p.IsFooterPartition()).Count();
            return count <= 1;
        }

        // Helpers
        public bool IsHeaderPartitionOpen()
        {
            if (IsHeaderPartitionPresent())
            {
                return !this.File.GetPartitions().First().IsClosed();
            }
            return false;
        }

        public bool AreBodyPartitionsPresent()
        {
            return File.GetBodyPartitions().Any();
        }

        public bool AreIndexTableSegmentsInBodyPartitions()
        {
            return this.File.GetBodyPartitions()?.Where(p => p.ContainsIndexTableSegments()).Any() ?? false;
        }

        public bool HeaderPartitionContainsIndexTableSegments()
        {
            if (IsHeaderPartitionPresent() && IsHeaderPartitionUnique())
            {
                return this.File.GetPartitions()?.First().ContainsIndexTableSegments() ?? false;
            }
            else return false;
        }

        public bool FooterPartitionContainsIndexTableSegments()
        {
            if (IsFooterPartitionPresent() && IsFooterPartitionUnique())
            {
                return this.File.GetPartitions()?.Last().Children.Where(c => c is MXFIndexTableSegment).Any() ?? false;
            }
            else return false;
        }

        public bool HeaderPartitionContainsMetadata()
        {
            if (IsHeaderPartitionPresent() && IsHeaderPartitionUnique())
            {
                return this.File.GetPartitions()?.First().Children.Any(c => c.IsMetadataLike()) ?? false;
            }
            else return false;
        }

        public bool IsOperationalPatternConsistent()
        {
            if (AnyPartitionsPresent())
            {
                return this.File.GetPartitions().GroupBy(p => p.OperationalPattern).Count() == 1;
            }
            else return false;
        }

        public bool IsMajorMinorVersionConsistent()
        {
            if (AnyPartitionsPresent())
            {
                return this.File.GetPartitions().GroupBy(p => new { p.MajorVersion, p.MinorVersion }).Count() == 1;
            }
            else return false;
        }
        public bool IsThisPartitionValueCorrect(MXFPartition p)
        {
            return p.ThisPartition + runInHeaderOffset == (ulong)p.Offset;
        }

        public bool IsPreviousPartitionValueCorrect(MXFPartition p)
        {
            if (p.PreviousSibling() is MXFPartition prev)
            {
                return p.PreviousPartition + runInHeaderOffset == (ulong)prev.Offset;
            }
            // previous partition is/points to header partition
            else return p.PreviousPartition == 0;
        }

        public bool IsFooterPartitionValueCorrect(MXFPartition p)
        {
            if (IsFooterPartitionPresent() && IsFooterPartitionUnique())
            {
                var footer = File.GetFooterPartition();

                // In Open Partitions, the value shall be as defined in Section 7.1 or zero(0).
                // If the Footer Partition is not present in the file then the value of this Property shall be zero(0).
                if (p.IsOpen())
                {
                    return (p.FooterPartition + runInHeaderOffset == (ulong)footer.Offset) || p.FooterPartition == 0;
                }
                return p.FooterPartition + runInHeaderOffset == (ulong)footer.Offset;
            }
            else
            {
                return p.FooterPartition == 0;
            }
        }

        public bool IsPartitionStatusValid(MXFPartition p)
        {
            return
                p.Status == PartitionStatus.OpenComplete ||
                p.Status == PartitionStatus.OpenIncomplete ||
                p.Status == PartitionStatus.ClosedIncomplete ||
                p.Status == PartitionStatus.ClosedComplete;
        }

        public bool MajorVersionEqualsTo(MXFPartition p, int versionNumber)
        {
            return p.MajorVersion == versionNumber;
        }

        public bool MinorVersionEqualsTo(MXFPartition p, int versionNumber)
        {
            // SMPTE 377:2011 requires Minor Version 1.3
            return p.MinorVersion == versionNumber;
        }


        public bool IsHeaderByteCountCorrect(MXFPartition p)
        {
            // according to SMPTE 377:2011 this is the Count of Bytes used for Header Metadata and
            // Primer Pack. This starts at the first byte of the key of the Primer Pack and ends after
            // any trailing KLV Fill item which is included within this HeaderByteCount.
            ulong headerByteCount = 0;

            var primerPack = p.Children.FirstOrDefault(c => c is MXFPrimerPack);
            var lastHeaderMetadata = p.Children.TakeWhile(c => c.IsHeaderMetadataLike() && !c.IsIndexLike())?.LastOrDefault();

            // TODO: what if there are two or more consecutive filler?
            if (primerPack != null && lastHeaderMetadata != null)
            {
                headerByteCount = (ulong)(lastHeaderMetadata.Offset + lastHeaderMetadata.TotalLength) - (ulong)primerPack.Offset;
            }

            return p.HeaderByteCount == headerByteCount;
        }

        public bool IsIndexByteCountCorrect(MXFPartition p)
        {
            ulong indexByteCount = 0;

            var indexTableSegments = p.Children.TakeWhile(c => c is MXFIndexTableSegment);

            if (indexTableSegments.Any())
            {
                indexByteCount = (ulong)(indexTableSegments.Last().Offset + indexTableSegments.Last().TotalLength) - (ulong)indexTableSegments.First().Offset;
            }

            if (indexTableSegments.Last().NextSibling() is MXFFillerData filler)
            {
                indexByteCount += indexByteCount + (ulong)filler.TotalLength;
            }

            // TODO what if there are two or more consecutive filler?

            return p.IndexByteCount == indexByteCount;
        }
        protected override async Task<List<MXFValidationResult>> OnValidate(IProgress<TaskReport> progress = null, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            List<MXFValidationResult> result = await Task.Run(() =>
            {
                var retval = new List<MXFValidationResult>();
                Stopwatch sw = Stopwatch.StartNew();

                this.Description = "Validating partitions";

                if (AnyPartitionsPresent())
                {
                    if (AreBodyPartitionsPresent())
                    {
                        retval.Add(ValidationRules.CreateValidationResult(
                            ValidationRuleIDs.ID_0054, 
                            File.GetPartitionRoot(), 
                            File.GetPartitionRoot().Offset, 
                            this.File.GetBodyPartitions().Count()));

                        if (AreIndexTableSegmentsInBodyPartitions())
                        {
                            retval.Add(ValidationRules.CreateValidationResult(
                                ValidationRuleIDs.ID_0052,
                                null,
                                0
                            ));
                        }

                        if (!IsOperationalPatternConsistent())
                        {
                            retval.Add(ValidationRules.CreateValidationResult(
                                ValidationRuleIDs.ID_0065,
                                File.GetPartitionRoot(),
                                File.GetPartitionRoot().Offset
                            ));
                        }

                        if (!IsMajorMinorVersionConsistent())
                        {
                            retval.Add(ValidationRules.CreateValidationResult(
                                ValidationRuleIDs.ID_0707,
                                File.GetPartitionRoot(),
                                File.GetPartitionRoot().Offset
                            ));
                        }
                    }

                    // Header partition checks

                    if (!IsHeaderPartitionUnique())
                    {
                        retval.Add(ValidationRules.CreateValidationResult(
                            ValidationRuleIDs.ID_0056,
                            null,
                            0
                        ));
                    }

                    if (!IsHeaderPartitionPresent())
                    {
                        var firstPartition = this.File.GetPartitions()?.First();
                        retval.Add(ValidationRules.CreateValidationResult(
                            ValidationRuleIDs.ID_0055,
                            firstPartition,
                            firstPartition?.Offset
                        ));
                    }
                    else
                    {
                        var firstPartition = this.File.GetPartitions()?.First();
                        if (IsHeaderPartitionOpen())
                        {
                            retval.Add(ValidationRules.CreateValidationResult(
                                ValidationRuleIDs.ID_0092,
                                firstPartition,
                                firstPartition?.Offset
                            ));
                        }

                        if (!HeaderPartitionContainsMetadata())
                        {
                            retval.Add(ValidationRules.CreateValidationResult(
                                ValidationRuleIDs.ID_0058,
                                firstPartition,
                                firstPartition?.Offset
                            ));
                        }

                        if (HeaderPartitionContainsIndexTableSegments())
                        {
                            retval.Add(ValidationRules.CreateValidationResult(
                                ValidationRuleIDs.ID_0051,
                                firstPartition,
                                firstPartition?.Offset
                            ));
                        }
                    }

                    // Footer partition checks

                    //  TODO check if at least two partitions
                    if (!IsFooterPartitionUnique())
                    {
                        retval.Add(ValidationRules.CreateValidationResult(
                            ValidationRuleIDs.ID_0057,
                            null,
                            0
                        ));
                    }

                    if (IsFooterPartitionPresent())
                    {
                        var lastPartition = this.File.GetPartitions().Last();
                        retval.Add(ValidationRules.CreateValidationResult(
                            ValidationRuleIDs.ID_0708,
                            lastPartition,
                            lastPartition.Offset
                        ));

                        if (FooterPartitionContainsIndexTableSegments())
                        {
                            retval.Add(ValidationRules.CreateValidationResult(
                                ValidationRuleIDs.ID_0053,
                                lastPartition,
                                lastPartition.Offset
                            ));
                        }

                        // If there is a footer then it must be closed
                        var footer = File.GetFooterPartition();
                        if (footer.IsOpen())
                        {
                            retval.Add(ValidationRules.CreateValidationResult(
                                ValidationRuleIDs.ID_0060,
                                footer,
                                footer.Offset
                            ));
                        }
                    }


                    // *****************************************************
                    // checks for all partitions 

                    foreach (var p in this.File.GetPartitions())
                    {
                        CheckPartitionProperties(retval, p);
                    }
                }
                else
                {
                    retval.Add(ValidationRules.CreateValidationResult(
                        ValidationRuleIDs.ID_0711,
                        null,
                        null
                    ));
                }

                Log.ForContext<MXFValidatorPartitions>().Information($"Validation completed in {sw.ElapsedMilliseconds} ms");

                return retval;
            }, ct);
            return result;
        }

        private void CheckPartitionProperties(List<MXFValidationResult> retval, MXFPartition p)
        {
            if (!IsThisPartitionValueCorrect(p))
            {
                retval.Add(ValidationRules.CreateValidationResult(
                    ValidationRuleIDs.ID_0095,
                    p,
                    p.Offset,
                    p.PartitionNumber
                ));
            }

            if (!IsPreviousPartitionValueCorrect(p))
            {
                retval.Add(ValidationRules.CreateValidationResult(
                    ValidationRuleIDs.ID_0096,
                    p,
                    p.Offset,
                    p.PartitionNumber
                ));
            }

            if (IsFooterPartitionPresent() && IsFooterPartitionUnique())
            {
                // TODO Consider if Partition is open
                if (!IsFooterPartitionValueCorrect(p))
                {
                    retval.Add(ValidationRules.CreateValidationResult(
                        ValidationRuleIDs.ID_0097,
                        p,
                        p.Offset,
                        p.PartitionNumber
                    ));
                }
            }
            else if (!IsFooterPartitionPresent() && p.FooterPartition != 0)
            {
                retval.Add(ValidationRules.CreateValidationResult(
                    ValidationRuleIDs.ID_0710,
                    p,
                    p.Offset,
                    p.PartitionNumber
                ));
            }

            if (!IsPartitionStatusValid(p))
            {
                retval.Add(ValidationRules.CreateValidationResult(
                    ValidationRuleIDs.ID_0114,
                    p,
                    p.Offset,
                    p.PartitionNumber,
                    (byte)p.Status
                ));
            }

            if (!MajorVersionEqualsTo(p, MAJOR_VERSION))
            {
                retval.Add(ValidationRules.CreateValidationResult(
                    ValidationRuleIDs.ID_0107,
                    p,
                    p.Offset,
                    p.PartitionNumber,
                    p.MajorVersion
                ));
            }

            if (!MinorVersionEqualsTo(p, MINOR_VERSION_SMPTE377M_2011))
            {
                retval.Add(ValidationRules.CreateValidationResult(
                    ValidationRuleIDs.ID_0108,
                    p,
                    p.Offset,
                    p.PartitionNumber,
                    p.MinorVersion
                ));
            }

            if (!IsHeaderByteCountCorrect(p))
            {
                retval.Add(ValidationRules.CreateValidationResult(
                    ValidationRuleIDs.ID_0098,
                    p,
                    p.Offset,
                    p.PartitionNumber
                ));
            }

            // TODO Check Index byte count

            // TODO Check body offset

            // TODO Check body SID

            // TODO Check index SID

            // TODO Check KAG size
        }
    }
}
