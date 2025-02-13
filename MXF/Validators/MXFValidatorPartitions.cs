﻿#region license
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

namespace Myriadbits.MXF
{
    public class MXFValidatorPartitions : MXFValidator
    {
        private ulong runInHeaderOffset = 0;

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

        public bool IsFooterPartitionPresent()
        {
            var lastPartition = this.File.GetPartitions()?.Last();
            return lastPartition != null && lastPartition.IsFooterPartition();
        }

        public bool IsHeaderPartitionPresent()
        {
            var firstPartition = this.File.GetPartitions()?.First();
            return firstPartition != null && firstPartition.IsHeaderPartition();
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

        public bool FooterPartitionContainsIndexTableSegments()
        {
            if (IsFooterPartitionPresent() && IsFooterPartitionUnique())
            {
                return this.File.GetPartitions()?.Last().Children.Where(c => c is MXFIndexTableSegment).Any() ?? false;
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
                if (IsOpen(p))
                {
                    return (p.FooterPartition + runInHeaderOffset == (ulong)footer.Offset)  || p.FooterPartition == 0;
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

        public bool IsOpen(MXFPartition p)
        {
            return
                p.Status == PartitionStatus.OpenComplete ||
                p.Status == PartitionStatus.OpenIncomplete;
        }
        public bool IsMajorVersionCorrect(MXFPartition p)
        {
            return p.MajorVersion == 1;
        }

        public bool IsMinorVersionCorrect(MXFPartition p)
        {
            // SMPTE 377:2011 requires Minor Version 1.3
            return p.MinorVersion == 3;
        }

        public bool IsHeaderByteCountCorrect(MXFPartition p)
        {
            // according to SMPTE 377:2011 this is the Count of Bytes used for Header Metadata and
            // Primer Pack. This starts at the first byte of the key of the Primer Pack and ends after
            // any trailing KLV Fill item which is included within this HeaderByteCount.
            ulong headerByteCount = 0;

            var primerPack = p.Children.FirstOrDefault(c => c is MXFPrimerPack);
            var lastHeaderMetadata = p.Children.TakeWhile(c => c.IsHeaderMetadataLike() && !c.IsIndexLike())?.LastOrDefault();

            if (primerPack != null && lastHeaderMetadata != null)
            {
                headerByteCount = (ulong)(lastHeaderMetadata.Offset + lastHeaderMetadata.TotalLength) - (ulong)primerPack.Offset;
            }

            return p.HeaderByteCount == headerByteCount;
        }

        public bool IsHeaderPartitionOpen()
        {
            if (IsHeaderPartitionPresent())
            {
                return !this.File.GetPartitions().First().IsClosed();
            }
            return false;
        }

        /// <summary>
        /// Check if partition structure and its values are correct 
        /// </summary>
        /// <param name="results"></param>
        protected override async Task<List<MXFValidationResult>> OnValidate(IProgress<TaskReport> progress = null, CancellationToken ct = default)
        {
            const string CATEGORY_NAME = "Partitions";
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
                        retval.Add(new MXFValidationResult
                        {
                            Category = CATEGORY_NAME,
                            Object = File.GetPartitionRoot(),
                            Severity = MXFValidationSeverity.Success,
                            Message = $"{this.File.GetBodyPartitions().Count()} Body Partition(s) present"
                        });

                        if (AreIndexTableSegmentsInBodyPartitions())
                        {
                            retval.Add(new MXFValidationResult
                            {
                                Category = CATEGORY_NAME,
                                Severity = MXFValidationSeverity.Success,
                                Message = $"At least one Body Partition contains Index Table Segments"
                            });
                        }

                        if (!IsOperationalPatternConsistent())
                        {
                            retval.Add(new MXFValidationResult
                            {
                                Category = CATEGORY_NAME,
                                Object = File.GetPartitionRoot(),
                                Severity = MXFValidationSeverity.Error,
                                Message = $"The Operational Pattern property is not consistent across all partitions"
                            });
                        }

                        if (!IsMajorMinorVersionConsistent())
                        {
                            retval.Add(new MXFValidationResult
                            {
                                Category = CATEGORY_NAME,
                                Object = File.GetPartitionRoot(),
                                Severity = MXFValidationSeverity.Error,
                                Message = $"The Major/Minor Version property is not consistent across all partitions"
                            });
                        }
                    }

                    // Header partition checks

                    if (!IsHeaderPartitionUnique())
                    {
                        retval.Add(new MXFValidationResult
                        {
                            Category = CATEGORY_NAME,
                            Offset = 0,
                            Severity = MXFValidationSeverity.Error,
                            Message = "Invalid partition structure. Only the first partition shall be a Header Partition"
                        });
                    }

                    if (!IsHeaderPartitionPresent())
                    {
                        retval.Add(new MXFValidationResult
                        {
                            Category = CATEGORY_NAME,
                            Object = this.File.GetPartitions()?.First(),
                            Severity = MXFValidationSeverity.Error,
                            Message = "Invalid partition structure. The first partition must be a Header Partition"
                        });
                    }
                    else
                    {
                        if (IsHeaderPartitionOpen())
                        {
                            retval.Add(new MXFValidationResult
                            {
                                Category = CATEGORY_NAME,
                                Object = this.File.GetPartitions()?.First(),
                                Severity = MXFValidationSeverity.Success,
                                Message = "Header Partition status is \"open\" (= required values may be absent)"
                            });
                        }

                        if (!HeaderPartitionContainsMetadata())
                        {
                            retval.Add(new MXFValidationResult
                            {
                                Category = CATEGORY_NAME,
                                Object = this.File.GetPartitions()?.First(),
                                Severity = MXFValidationSeverity.Error,
                                Message = "Header Partition does not contain header metadata"
                            });
                        }

                        if (HeaderPartitionContainsIndexTableSegments())
                        {
                            retval.Add(new MXFValidationResult
                            {
                                Category = CATEGORY_NAME,
                                Object = this.File.GetPartitions()?.First(),
                                Severity = MXFValidationSeverity.Success,
                                Message = $"Header Partition contains Index Table Segments"
                            });
                        }
                    }

                    // Footer partition checks

                    //  TODO check if at least two partitions
                    if (!IsFooterPartitionUnique())
                    {
                        retval.Add(new MXFValidationResult
                        {
                            Category = CATEGORY_NAME,
                            Offset = 0,
                            Severity = MXFValidationSeverity.Error,
                            Message = "Invalid partition structure. Only the last partition can be a Footer Partition"
                        });
                    }

                    if (IsFooterPartitionPresent())
                    {
                        retval.Add(new MXFValidationResult
                        {
                            Category = CATEGORY_NAME,
                            Object = this.File.GetPartitions().Last(),
                            Severity = MXFValidationSeverity.Success,
                            Message = "Footer Partition present"
                        });

                        if (FooterPartitionContainsIndexTableSegments())
                        {
                            retval.Add(new MXFValidationResult
                            {
                                Category = CATEGORY_NAME,
                                Object = this.File.GetPartitions().Last(),
                                Severity = MXFValidationSeverity.Success,
                                Message = $"Footer Partition contains Index Table Segments"
                            });
                        }

                        // If there is a footer then it must be closed
                        var footer = File.GetFooterPartition();
                        if (IsOpen(footer))
                        {
                            retval.Add(new MXFValidationResult
                            {
                                Category = CATEGORY_NAME,
                                Object = footer,
                                Severity = MXFValidationSeverity.Success,
                                Message = $"The Footer Partition, if present, must be closed"
                            });
                        }
                    }

                    // *****************************************************
                    // checks for all partitions 

                    foreach (var p in this.File.GetPartitions())
                    {
                        if (!IsThisPartitionValueCorrect(p))
                        {
                            retval.Add(new MXFValidationResult
                            {
                                Category = CATEGORY_NAME,
                                Object = p,
                                Severity = MXFValidationSeverity.Error,
                                Message = $"Partition #{p.PartitionNumber} has incorrect value for ThisPartition"
                            });
                        }

                        if (!IsPreviousPartitionValueCorrect(p))
                        {
                            retval.Add(new MXFValidationResult
                            {
                                Category = CATEGORY_NAME,
                                Object = p,
                                Severity = MXFValidationSeverity.Error,
                                Message = $"Partition #{p.PartitionNumber} has incorrect value for PreviousPartition"
                            });
                        }

                        if (IsFooterPartitionPresent() && IsFooterPartitionUnique())
                        {
                            // TODO Consider if Partition is open
                            if (!IsFooterPartitionValueCorrect(p))
                            {
                                retval.Add(new MXFValidationResult
                                {
                                    Category = CATEGORY_NAME,
                                    Object = p,
                                    Severity = MXFValidationSeverity.Error,
                                    Message = $"Partition #{p.PartitionNumber} has incorrect value for FooterPartition"
                                });
                            }
                        }
                        else if (!IsFooterPartitionPresent() && p.FooterPartition != 0)
                        {
                            retval.Add(new MXFValidationResult
                            {
                                Category = CATEGORY_NAME,
                                Object = p,
                                Severity = MXFValidationSeverity.Error,
                                Message = $"FooterPartition value of Partition #{p.PartitionNumber} is non-zero but the file has no Footer Partition"
                            });
                        }

                        if (!IsPartitionStatusValid(p))
                        {
                            retval.Add(new MXFValidationResult
                            {
                                Category = CATEGORY_NAME,
                                Object = p,
                                Severity = MXFValidationSeverity.Error,
                                Message = $"Partition #{p.PartitionNumber} has an invalid PartitionStatus (0x{(byte)p.Status:X2})"
                            });
                        }

                        if (!IsMajorVersionCorrect(p))
                        {
                            retval.Add(new MXFValidationResult
                            {
                                Category = CATEGORY_NAME,
                                Object = p,
                                Severity = MXFValidationSeverity.Warning,
                                Message = $"Partition #{p.PartitionNumber} Major Version property has an invalid value (read: {p.MajorVersion}, expected: 1)"
                            });
                        }

                        if (!IsMinorVersionCorrect(p))
                        {
                            retval.Add(new MXFValidationResult
                            {
                                Category = CATEGORY_NAME,
                                Object = p,
                                Severity = MXFValidationSeverity.Warning,
                                Message = $"Partition #{p.PartitionNumber} Major Version property has an invalid value (read: {p.MinorVersion}, expected: 3)"
                            });
                        }

                        if (!IsHeaderByteCountCorrect(p))
                        {
                            retval.Add(new MXFValidationResult
                            {
                                Category = CATEGORY_NAME,
                                Object = p,
                                Severity = MXFValidationSeverity.Error,
                                Message = $"Partition #{p.PartitionNumber} has incorrect value for HeaderByteCount"
                            });
                        }
                    }
                }
                else
                {
                    retval.Add(new MXFValidationResult
                    {
                        Category = CATEGORY_NAME,
                        // TODO make Offset nullable?
                        Offset = 0,
                        Severity = MXFValidationSeverity.Error,
                        Message = "No partitions detected"
                    });
                }

                Log.ForContext<MXFValidatorPartitions>().Information($"Validation completed in {sw.ElapsedMilliseconds} ms");

                return retval;
            }, ct);
            return result;
        }


    }
}
