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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

namespace Myriadbits.MXF
{
    public class MXFValidatorIndex : MXFValidator
    {
        private List<MXFIndexTableSegment> m_indexTables = new List<MXFIndexTableSegment>();
        private List<MXFSystemMetaDataPack> m_systemItems = new List<MXFSystemMetaDataPack>();
        private List<MXFEssenceElement> m_pictureItems = new List<MXFEssenceElement>();

        public MXFValidatorIndex(MXFFile file) : base(file)
        {

        }

        protected override async Task<List<MXFValidationResult>> OnValidate(IProgress<TaskReport> progress = null, CancellationToken ct = default)
        {
            List<MXFValidationResult> result = await Task.Run(() =>
            {
                var retval = new List<MXFValidationResult>();

                Stopwatch sw = Stopwatch.StartNew();
                progress?.Report(new TaskReport(12, "Locating index tables"));

                CreateIndexTableDictionary();

                var dict = GetEssencesWithEssenceOffset(ct);

                // Clear list
                m_indexTables = new List<MXFIndexTableSegment>();
                m_systemItems = new List<MXFSystemMetaDataPack>();
                m_pictureItems = new List<MXFEssenceElement>();

                //LoadAllPartitions();

                FindIndexTablesSystemItemsEssenceElements();

                progress?.Report(new TaskReport(55, "Checking indices"));
                Log.ForContext<MXFValidatorIndex>().Information($"Found {this.m_indexTables.Count} index table segments in {sw.ElapsedMilliseconds} ms");
                sw.Restart();

                // Check if first index table is CBE
                if (this.m_indexTables.Count == 0)
                {
                    retval.Add(new MXFValidationResult
                    {
                        Category = "Index Tables",
                        Severity = MXFValidationSeverity.Warning,
                        Message = "No index table segments found"
                    });
                    return retval;
                }


                MXFIndexTableSegment firstTable = this.m_indexTables.FirstOrDefault();
                if (firstTable != null)
                {
                    // is Index table CBE or VBE?
                    if (firstTable.EditUnitByteCount > 0)
                    {
                        // Check other tables
                        for (int n = 1; n < this.m_indexTables.Count; n++)
                        {
                            if (this.m_indexTables[n].EditUnitByteCount != firstTable.EditUnitByteCount &&
                                this.m_indexTables[n].BodySID == firstTable.BodySID)
                            {
                                retval.Add(new MXFValidationResult
                                {
                                    Category = "Index Tables",
                                    Severity = MXFValidationSeverity.Error,
                                    Message = $"Constant Bytes per Element ({firstTable.EditUnitByteCount} bytes) but index table {n} has other CBE: {this.m_indexTables[n].EditUnitByteCount} (both have BodySID {firstTable.BodySID})."
                                });
                                return retval;
                            }
                        }

                        // CBE for this BodySID
                        if (firstTable.IndexDuration == 0 && firstTable.IndexStartPosition == 0)
                        {
                            retval.Add(new MXFValidationResult
                            {
                                Category = "Index Tables",
                                Severity = MXFValidationSeverity.Success,
                                Message = $"Constant Bytes per Element ({firstTable.EditUnitByteCount} bytes) for the whole file for BodySID {firstTable.BodySID}"
                            });
                            // TODO Check if the size of all compounds is the same??
                            return retval;
                        }
                        else
                        {
                            // Complicated, mixed file
                            retval.Add(new MXFValidationResult
                            {
                                Category = "Index Tables",
                                Severity = MXFValidationSeverity.Warning,
                                Message = $"Constant Bytes per Element ({firstTable.EditUnitByteCount} bytes) but not for the whole file for BodySID {firstTable.BodySID}. Duration: {firstTable.IndexDuration}, StartOffset: {firstTable.IndexStartPosition}"
                            });
                            return retval;
                        }
                    }
                }

                // Check if there are multiple index table segments pointing to the same data
                int invalidCt = 0;
                int validCt = 0;
                int totalSystemItems = 0;
                int counter = 0;
                this.Description = "Checking for duplicates";
                foreach (MXFIndexTableSegment ids in this.m_indexTables)
                {
                    progress?.Report(new TaskReport(55 + (counter * 20) / this.m_indexTables.Count(), "Checking indices"));

                    // And all index entries
                    if (ids.IndexEntries != null)
                    {
                        ct.ThrowIfCancellationRequested();
                        List<MXFIndexTableSegment> sameStuff = this.m_indexTables.Where(a => a != ids && a.IndexSID == ids.IndexSID && a.IndexStartPosition == ids.IndexStartPosition).ToList();
                        foreach (MXFIndexTableSegment sameIds in sameStuff)
                        {
                            if (sameIds.IndexEntries.Count != ids.IndexEntries.Count)
                            {
                                retval.Add(new MXFValidationResult
                                {
                                    Category = "Index Tables",
                                    Severity = MXFValidationSeverity.Error,
                                    Message = $"Index {sameIds.IndexSID} in partition {sameIds.Partition} is not the same length as in partition {ids.Partition}!"
                                });
                                invalidCt++;
                            }
                            else
                            {
                                for (int n = 0; n < sameIds.IndexEntries.Count; n++)
                                {
                                    if (ids.IndexEntries[n].KeyFrameOffset != sameIds.IndexEntries[n].KeyFrameOffset ||
                                        ids.IndexEntries[n].StreamOffset != sameIds.IndexEntries[n].StreamOffset ||
                                        ids.IndexEntries[n].TemporalOffset != sameIds.IndexEntries[n].TemporalOffset)
                                    {
                                        retval.Add(new MXFValidationResult
                                        {
                                            Category = "Index Tables",
                                            Severity = MXFValidationSeverity.Error,
                                            Message = $"The indexentry {n} of Index {sameIds.IndexSID} in partition {sameIds.Partition.ToString()} is not the same data as in partition {ids.Partition.ToString()}!"
                                        });
                                        invalidCt++;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    counter++;
                }


                // First try to match system items
                if (m_systemItems.Count != 0)
                {
                    // For each index table segment				
                    totalSystemItems = m_systemItems.Count();
                    counter = 0;
                    this.Description = "Checking essence offsets";
                    foreach (MXFIndexTableSegment ids in this.m_indexTables)
                    {
                        progress?.Report(new TaskReport(75 + (counter * 20) / this.m_indexTables.Count(), "Checking system items"));

                        // And all index entries
                        if (ids.IndexEntries != null)
                        {
                            foreach (MXFEntryIndex index in ids.IndexEntries)
                            {
                                // Check if there is a system item at this offset
                                long searchIndex = (long)(index.StreamOffset);
                                MXFSystemMetaDataPack si = this.m_systemItems.Where(a => GetEssenceOffset(a) == searchIndex).FirstOrDefault();
                                if (si != null)
                                {
                                    // Yes, found
                                    validCt++;
                                    si.Indexed = true;
                                }
                                else
                                {
                                    // Not found
                                    retval.Add(new MXFValidationResult
                                    {
                                        Category = "Index Tables",
                                        Severity = MXFValidationSeverity.Error,
                                        Message = $"Index {index.Index} not pointing to a valid picture essence!"
                                    });
                                    invalidCt++;
                                }
                            }
                        }
                        counter++;
                    }
                }
                // TODO else???
                else if (m_pictureItems.Count != 0)
                {
                    // Now try to match the picture essences

                    // For each index table segment				
                    counter = 0;
                    totalSystemItems = m_pictureItems.Count();
                    this.Description = "Checking picture offsets";
                    foreach (MXFIndexTableSegment ids in this.m_indexTables)
                    {
                        progress?.Report(new TaskReport(75 + (counter * 20) / this.m_indexTables.Count(), ""));

                        // And all index entries
                        if (ids.IndexEntries != null)
                        {
                            foreach (MXFEntryIndex index in ids.IndexEntries)
                            {
                                // Check if there is a system item at this offset
                                long searchIndex = (long)(index.StreamOffset);
                                MXFEssenceElement ee = this.m_pictureItems.Where(a => a.EssenceOffset == searchIndex).FirstOrDefault();
                                if (ee != null)
                                {
                                    // Yes, found
                                    validCt++;
                                    ee.Indexed = true;
                                }
                                else
                                {
                                    // Not found
                                    retval.Add(new MXFValidationResult
                                    {
                                        Category = "Index Tables",
                                        Severity = MXFValidationSeverity.Error,
                                        Message = $"Index {index.Index} not pointing to a valid picture essence!"
                                    });
                                    invalidCt++;
                                }
                            }
                        }
                    }
                }
                else
                {
                    retval.Add(new MXFValidationResult
                    {
                        Category = "Index Tables",
                        Severity = MXFValidationSeverity.Error,
                        Message = $"No system items and/or picture essences found (found {{m_indexTables.Count}} index table segments)"
                    });
                    return retval;
                }


                bool fError = false;
                if (this.m_systemItems.Count() > 0)
                {
                    if (this.m_systemItems.Count(a => !a.Indexed) != 0)
                    {
                        // Hmm still some items left in the array
                        retval.Add(new MXFValidationResult
                        {
                            Category = "Index Tables",
                            Severity = MXFValidationSeverity.Error,
                            Message = $"There are {this.m_systemItems.Count} essence elements(of the total {totalSystemItems}) that are not referenced in an index table!"
                        });
                        fError = true;
                    }
                }
                else if (this.m_pictureItems.Count() > 0)
                {
                    if (this.m_pictureItems.Count(a => !a.Indexed) != 0)
                    {
                        // Hmm still some items left in the array
                        retval.Add(new MXFValidationResult
                        {
                            Category = "Index Tables",
                            Severity = MXFValidationSeverity.Error,
                            Message = $"There are {this.m_pictureItems.Count} essence elements (of the total {totalSystemItems}) that are not referenced in an index table!",
                        });
                        fError = true;
                    }
                }


                if (!fError)
                {
                    if (invalidCt > 0)
                        retval.Add(new MXFValidationResult
                        {
                            Category = "Index Tables",
                            Severity = MXFValidationSeverity.Error,
                            Message = $"Found {invalidCt} index errors! There are {invalidCt} indices that are NOT pointing to valid essence data (valid {validCt})!"

                        });
                    else if (validCt > 0)
                        retval.Add(new MXFValidationResult
                        {
                            Category = "Index Tables",
                            Severity = MXFValidationSeverity.Success,
                            Message = $"Index table is valid! All {validCt} index entries point to valid essences!"

                        });
                    if (validCt == 0 && invalidCt == 0)
                        retval.Add(new MXFValidationResult
                        {
                            Category = "Index Tables",
                            Severity = MXFValidationSeverity.Error,
                            Message = $"No valid indexes found in this file!"

                        });
                }
                Log.ForContext<MXFValidatorIndex>().Information($"Validation completed in {sw.ElapsedMilliseconds} ms");


                // Check system item range
                if (this.m_systemItems.Count() > 0)
                {
                    retval.AddRange(CheckUserDates(this.File));
                    retval.AddRange(CheckContinuityCounter(this.File));
                }
                return retval;
            }, ct);
            return result;
        }
        private void FindIndexTablesSystemItemsEssenceElements()
        {
            // Find all index tables, system items and essence elements
            this.Description = "Locating index tables, essence";

            m_indexTables = this.File.GetDescendantsOfType<MXFIndexTableSegment>().ToList();
            m_systemItems = this.File.GetDescendantsOfType<MXFSystemMetaDataPack>().ToList();
            m_pictureItems = this.File.GetDescendantsOfType<MXFEssenceElement>().Where(e => e.IsPicture).ToList();

            foreach (var si in m_systemItems)
            {
                si.Indexed = false;
            }

            foreach (var pe in m_pictureItems)
            {
                pe.Indexed = false;
            }
        }

        private Dictionary<(uint, long), MXFPack> GetEssencesWithEssenceOffset(CancellationToken ct = default)
        {
            // Collect essences in single list
            IEnumerable<MXFEssenceElement> essences = File.Descendants().OfType<MXFEssenceElement>();
            IEnumerable<MXFSystemMetaDataPack> systemMetadDataPacks = File.Descendants().OfType<MXFSystemMetaDataPack>();
            IEnumerable<MXFSystemMetaDataSet> systemMetaDataSets = File.Descendants().OfType<MXFSystemMetaDataSet>();

            List<MXFPack> essenceList = new List<MXFPack>();

            essenceList.AddRange(essences);
            essenceList.AddRange(systemMetadDataPacks);
            essenceList.AddRange(systemMetaDataSets);

            //// group by BodySID
            //var groups = essenceList.GroupBy(e => GetBodySID(e));

            ////foreach (var g in groups)
            ////{
            ////    List<Dictionary<long, MXFPack>> dicts = new List<Dictionary<long, MXFPack>>();

            ////    dicts.Add()
            ////}

            Dictionary<(uint, long), MXFPack> dict = new Dictionary<(uint, long), MXFPack>();

            foreach (var el in essenceList)
            {
                ct.ThrowIfCancellationRequested();
                var essenceOffset = GetEssenceOffset(el);
                var bodySID = GetBodySID(el);
                if (essenceOffset != null)
                {
                    try
                    {
                        dict.Add((bodySID.Value, essenceOffset.Value), el);
                        //Debug.WriteLine(dict.Last());
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            return dict;
        }


        private void CreateIndexTableDictionary()
        {
            var indexTables = this.File.GetDescendantsOfType<MXFIndexTableSegment>();

            foreach (var table in indexTables)
            {
                if (table.BodySID != null)
                {
                    var essences = GetEssencesWithBodySID(table.BodySID.Value);
                    var groups = essences.GroupBy(e => e.Key);
                    var elementCount = groups.Count();
                    var essenceDict = essences.ToDictionary(e => GetEssenceOffset(e));

                    if (table.EditUnitByteCount > 0)
                    {
                        // CBE
                        // check only delta entries
                        // assume that the SMPTE 379M (Generic Container Specification) is fullfilled, i.e. the
                        // order of the elementss shall always be the same in the stored file, so that it agrees
                        // with the order of the Delta entries.
                        // TODO this should be checked BEFORE any IndexValidator is called!!!

                        // first deltra entry must match with first essence element




                    }
                    else
                    {
                        // VBE
                    }
                }
                else
                {
                    // return ValidationError
                }
                // get essences  

            }
        }


        private IEnumerable<MXFPack> GetEssencesWithBodySID(uint bodySID)
        {
            return File
                .GetPartitions().Where(p => p.BodySID == bodySID)
                .SelectMany(p => p.Children)
                .Where(c => c is MXFEssenceElement || c is MXFSystemMetaDataPack || c is MXFSystemMetaDataSet)
                .OfType<MXFPack>();
        }

        private uint? GetBodySID(MXFPack pack)
        {
            if (pack.Parent is MXFPartition p)
            {
                return p.BodySID;
            }
            else return null;
        }

        private long? GetEssenceOffset(MXFObject obj)
        {

            if (obj.Parent is MXFPartition p)
            {
                MXFObject firstEssence = p.Children
                    .FirstOrDefault(c => c is MXFEssenceElement || c is MXFSystemMetaDataPack || c is MXFSystemMetaDataSet);
                if (firstEssence != null)
                {
                    return obj.Offset - firstEssence.Offset + ((long)p.BodyOffset);
                }
                else return null;
            }
            else return null;
        }

        //private void LoadAllPartitions()
        //{
        //    // Load all partitions
        //    this.Description = "Loading partitions";
        //    List<MXFPartition> partitionsToLoad = this.File.Partitions.Where(a => !a.IsLoaded).ToList();
        //    for (int n = 0; n < partitionsToLoad.Count(); n++)
        //    {
        //        int progress = (n * 50) / partitionsToLoad.Count();
        //        ReportProgress(progress);
        //        partitionsToLoad[n].Load();
        //    }
        //}


        /// <summary>
        /// Check the essence range
        /// </summary>
        /// <param name="file"></param>
        /// <param name="results"></param>
        protected List<MXFValidationResult> CheckUserDates(MXFFile file)
        {
            var retval = new List<MXFValidationResult>();

            List<MXFSystemMetaDataPack> items = this.m_systemItems.OrderBy(a => a.ContinuityCount).ToList();
            if (items.Count > 1)
            {
                MXFTimeStamp ts = new MXFTimeStamp(items.First().UserDate);
                if (ts != null)
                {
                    MXFTimeStamp tsLast = null;
                    for (int n = 1; n < items.Count() - 1; n++) // Skip last one (always invalid??)
                    {
                        ts.Increase();
                        if (!items[n].UserDate.IsEmpty())
                        {
                            if (!items[n].UserDate.IsSame(ts))
                            {
                                retval.Add(new MXFValidationResult
                                {
                                    Category = "Index Tables",
                                    Severity = MXFValidationSeverity.Error,
                                    Message = $"Invalid user date at offset {items[n].Offset} (was {items[n].UserDate}, expected {ts})!"

                                });
                                return retval;
                            }
                            tsLast = items[n].UserDate;
                        }
                    }
                    if (tsLast != null)
                        retval.Add(new MXFValidationResult
                        {
                            Category = "Index Tables",
                            Severity = MXFValidationSeverity.Success,
                            Message = $"UserDates are continious from {items.First().UserDate} to {tsLast}, at {ts.FrameRate} fps!"

                        });
                    else
                        retval.Add(new MXFValidationResult
                        {
                            Category = "Index Tables",
                            Severity = MXFValidationSeverity.Success,
                            Message = $"UserDates are continious!"

                        });
                }
            }
            return retval;
        }


        /// <summary>
        /// Check the essence range
        /// </summary>
        /// <param name="file"></param>
        /// <param name="results"></param>
        protected List<MXFValidationResult> CheckContinuityCounter(MXFFile file)
        {
            var retval = new List<MXFValidationResult>();

            // Check for continous range
            // Continuity count = modulo 65536 count as per SMPTE 326M. Note that the continuity
            // count is not strictly required in many applications of an MXF file because the header metadata
            // should correctly describe the timeline of the essence container. However, to maintain
            // compatibility with the SDTI-CP system item definition, the continuity count must comply
            // with SMPTE 326M.

            int cc = -1;
            int errorCount = 0;
            this.m_systemItems = this.m_systemItems.OrderBy(si => si.ContinuityCount).ToList();
            foreach (MXFSystemMetaDataPack si in this.m_systemItems)
            {
                if (si.ContinuityCount - cc != 1)
                {
                    errorCount++;
                    //valResult.SetError(string.Format("Invalid continuity count for system item at offset {0}. CC should be {1} but is {2}!", si.Offset, cc + 1, si.ContinuityCount));
                    //return;
                }
                cc = si.ContinuityCount;
            }

            if (errorCount > 0)
            {
                if (errorCount >= this.m_systemItems.Count() - 1)
                    retval.Add(new MXFValidationResult
                    {
                        Category = "Index Tables",
                        Severity = MXFValidationSeverity.Warning,
                        Message = "All continuity counter values are not set!"

                    });
                else
                    retval.Add(new MXFValidationResult
                    {
                        Category = "Index Tables",
                        Severity = MXFValidationSeverity.Warning,
                        Message = $"Found {errorCount} invalid continuity counter values (total system items {this.m_systemItems.Count()})!"

                    });
            }
            else
            {
                retval.Add(new MXFValidationResult
                {
                    Category = "Index Tables",
                    Severity = MXFValidationSeverity.Success,
                    Message = "Continuity counter values are correct!"

                });
            }

            return retval;
        }

    }
}
