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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Myriadbits.MXF.KLV;

namespace Myriadbits.MXF
{
    [ULGroup("urn:smpte:ul:060e2b34.027f0101.0d010201.01100100")]
    public class MXFIndexTableSegment : MXFInterchangeObject
    {
        private const string CATEGORYNAME = "IndexTableSegment";

        [Category(CATEGORYNAME)]
        [ULElement("urn:smpte:ul:060e2b34.01010104.01030404.00000000")]
        public UInt32? BodySID { get; set; }

        [Category(CATEGORYNAME)]
        [ULElement("urn:smpte:ul:060e2b34.01010104.01030405.00000000")]
        public UInt32? IndexSID { get; set; }

        [Category(CATEGORYNAME)]
        [ULElement("urn:smpte:ul:060e2b34.01010104.04040401.01000000")]
        public byte? SliceCount { get; set; }

        [Category(CATEGORYNAME)]
        [ULElement("urn:smpte:ul:060e2b34.01010105.04040401.07000000")]
        public byte? PositionTableCount { get; set; }

        [Category(CATEGORYNAME)]
        [ULElement("urn:smpte:ul:060e2b34.0101010e.04040501.00000000")]
        public bool? SingleIndexLocation { get; set; }

        [Category(CATEGORYNAME)]
        [ULElement("urn:smpte:ul:060e2b34.0101010e.04040502.00000000")]
        public bool? ForwardIndexDirection { get; set; }

        [Category(CATEGORYNAME)]
        [ULElement("urn:smpte:ul:060e2b34.01010104.04060201.00000000")]
        public UInt32? EditUnitByteCount { get; set; }

        [Category(CATEGORYNAME)]
        [ULElement("urn:smpte:ul:060e2b34.0101010a.04060204.00000000")]
        public UInt64? ExtStartOffset { get; set; }

        [Category(CATEGORYNAME)]
        [ULElement("urn:smpte:ul:060e2b34.0101010a.04060205.00000000")]
        public UInt64? VBEByteCount { get; set; }

        [Category(CATEGORYNAME)]
        [ULElement("urn:smpte:ul:060e2b34.0101010e.04060206.00000000")]
        public bool? SingleEssenceLocation { get; set; }

        [Category(CATEGORYNAME)]
        [ULElement("urn:smpte:ul:060e2b34.01010105.05300406.00000000")]
        public MXFRational IndexEditRate { get; set; }

        [Category(CATEGORYNAME)]
        [ULElement("urn:smpte:ul:060e2b34.01010105.07020103.010a0000")]
        public MXFPosition? IndexStartPosition { get; set; }

        [Category(CATEGORYNAME)]
        [ULElement("urn:smpte:ul:060e2b34.01010105.07020201.01020000")]
        public MXFPosition? IndexDuration { get; set; }

        // TODO helper property for indexvalidator that should be avoided
        [Browsable(false)]
        public List<MXFEntryIndex> IndexEntries { get; set; }


        public MXFIndexTableSegment(MXFPack pack)
            : base(pack, "IndexTableSegment") //base(pack, "IndexTableSegment", KeyType.IndexSegment)
        {
        }


        /// <summary>
        /// Overridden method to process local tags
        /// </summary>
        /// <param name="localTag"></param>
        protected override bool ReadLocalTagValue(IKLVStreamReader reader, MXFLocalTag localTag)
        {
            switch (localTag.TagValue)
            {
                case 0x3F05:
                    this.EditUnitByteCount = reader.ReadUInt32();
                    localTag.Value = this.EditUnitByteCount;
                    return true;
                case 0x3F06:
                    this.IndexSID = reader.ReadUInt32();
                    localTag.Value = this.IndexSID;
                    return true;
                case 0x3F07:
                    this.BodySID = reader.ReadUInt32();
                    localTag.Value = this.BodySID;
                    return true;
                case 0x3F08:
                    this.SliceCount = reader.ReadByte();
                    localTag.Value = this.SliceCount;
                    return true;
                case 0x3F0C:
                    this.IndexStartPosition = reader.ReadUInt64();
                    localTag.Value = this.IndexStartPosition;
                    return true;
                case 0x3F0D:
                    this.IndexDuration = reader.ReadUInt64();
                    localTag.Value = this.IndexDuration;
                    return true;
                case 0x3F0E:
                    this.PositionTableCount = reader.ReadByte();
                    localTag.Value = this.PositionTableCount;
                    return true;
                case 0x3F0F:
                    this.ExtStartOffset = reader.ReadUInt64();
                    localTag.Value = this.ExtStartOffset;
                    return true;
                case 0x3F10:
                    this.VBEByteCount = reader.ReadUInt64();
                    localTag.Value = this.VBEByteCount;
                    return true;
                case 0x3F11:
                    this.SingleIndexLocation = reader.ReadBoolean();
                    localTag.Value = this.SingleIndexLocation;
                    return true;
                case 0x3F12:
                    this.SingleEssenceLocation = reader.ReadBoolean();
                    localTag.Value = this.SingleEssenceLocation;
                    return true;
                case 0x3F13:
                    this.ForwardIndexDirection = reader.ReadBoolean();
                    localTag.Value = this.ForwardIndexDirection;
                    return true;
                case 0x3F0B:
                    this.IndexEditRate = reader.ReadRational();
                    localTag.Value = this.IndexEditRate;
                    return true;
                case 0x3F0A:  // Index entry array
                    {

                        // TODO use reader function to check the set size
                        UInt32 NbIndexEntries = reader.ReadUInt32();
                        UInt32 entryLength = reader.ReadUInt32();
                        if (NbIndexEntries > 0)
                        {
                            this.IndexEntries = new List<MXFEntryIndex>();
                            MXFObject indexCollection = new MXFObjectCollection("IndexEntries", localTag.Offset);

                            for (UInt64 i = 0; i < NbIndexEntries; i++)
                            {
                                long next = reader.Position + entryLength;
                                long offset = localTag.Offset + reader.Position;

                                MXFEntryIndex newEntry = new MXFEntryIndex((ulong)this.IndexStartPosition + i, reader, offset, this.SliceCount, this.PositionTableCount, entryLength);
                                this.IndexEntries.Add(newEntry); // Also add this entry to the local list

                                // And to the child collection
                                indexCollection.AddChild(newEntry);

                                reader.Seek(next);
                            }
                            localTag.AddChild(indexCollection);
                        }
                    }
                    return true;

                case 0x3F09:  // Delta entry array
                    {
                        // TODO use reader function to check the set size
                        UInt32 NbDeltaEntries = reader.ReadUInt32();
                        UInt32 entryLength = reader.ReadUInt32();
                        if (NbDeltaEntries > 0)
                        {
                            MXFObject deltaCollection = new MXFObjectCollection("DeltaEntries", localTag.Offset);
                            for (int i = 0; i < NbDeltaEntries; i++)
                            {
                                long next = reader.Position + entryLength;
                                long offset = localTag.Offset + reader.Position;

                                deltaCollection.AddChild(new MXFEntryDelta(reader, offset, entryLength));
                                reader.Seek(next);
                            }
                            localTag.AddChild(deltaCollection);
                        }
                    }
                    return true;
            }
            return base.ReadLocalTagValue(reader, localTag);
        }


        public override string ToString()
        {
            return string.Format("Index Table Segment [{0}], BodySID {1}", this.IndexSID, this.BodySID);
        }
    }
}
