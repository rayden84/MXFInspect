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
using System.Collections.Immutable;
using System.ComponentModel;

namespace Myriadbits.MXF.Identifiers
{
    public class UL : AUID
    {
        private const string CATEGORYNAME = "Key";
        public static readonly ImmutableArray<byte> ValidULPrefix = ImmutableArray.Create<byte>(0x06, 0x0e, 0x2b, 0x34);
        public static readonly ImmutableArray<byte> ValidPartitionPrefix = ImmutableArray.Create<byte>(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x05, 0x01, 0x01, 0x0d, 0x01, 0x02);
        private static readonly IReadOnlyDictionary<byte, string> privateOrganizationsIdentifiers = new Dictionary<byte, string>
        {
           {0x04,"Avid Technology, Inc." },
           {0x05,"CNN"},
           {0x06,"Sony Corporation"},
           {0x07,"IdeasUnlimited.TV"},
           {0x08,"IPV Ltd"},
           {0x09,"Dolby Laboratories Inc."},
           {0x0a,"Snell &Wilcox"},
           {0x0b,"Omneon Video Networks"},
           {0x0c,"Ascent Media Group, Inc."},
           {0x0d,"Quantel Ltd"},
           {0x0e,"Panasonic"},
           {0x0f,"Grass Valley, Inc."},
           {0x10,"Doremi Labs, Inc."},
           {0x11,"EVS Broadcast Equipment"},
           {0x12,"Turner Broadcasting System, Inc."},
           {0x13,"NL Technology, LLC"},
           {0x14,"Harris Corporation"},
           {0x15,"Canon, Inc."},
           {0x16,"D-BOX Technologies"},
           {0x17,"ARRI"},
           {0x18,"JVC"},
           {0x19,"3ality Technica"},
           {0x1a,"NHK"},
           {0x1b,"HBO"},
           {0x1d,"DTS, Inc."},
           {0x1e,"FLIR Systems, Inc."},
           {0x1f,"Barco"},
           {0x20,"Apple Inc."},
           {0x21,"Fraunhofer"},
        };

        #region properties

        [Category(CATEGORYNAME)]
        [Description("Identifies the category of registry described (e.g. Dictionaries)")]
        [TypeConverter(typeof(EnumDescriptionConverter))]
        public ULCategories? CategoryDesignator { get; private set; }

        [Category(CATEGORYNAME)]
        [Description("Identifies the specific register in a category (e.g. Metadata Dictionaries)")]
        [TypeConverter(typeof(EnumDescriptionConverter))]
        public ULRegistries? RegistryDesignator { get; private set; }

        [Category(CATEGORYNAME)]
        [Description("Designator of the structure variant within the given registry designator")]
        public byte StructureDesignator { get; private set; }

        [Category(CATEGORYNAME)]
        [Description("Version of the given register which first defines the item specified by the Item Designator")]
        public byte VersionNumber { get; private set; }

        [Category(CATEGORYNAME)]
        [Description("Unique identification of the particular item within the context of the UL Designator")]
        [TypeConverter(typeof(ByteArrayConverter))]
        public byte[] ItemDesignator { get; private set; }
        #endregion

        [Category(CATEGORYNAME)]
        [Description("SMPTE Information")]
        public ULDescription SMPTEInformation { get; }

        [Browsable(false)]
        [Description("Universal Label Name")]
        public string Name { get; set; }

        public UL(params byte[] bytes) : base(bytes)
        {
            if (!HasValidULPrefix(bytes))
            {
                throw new ArgumentException("Wrong byte value. A SMPTE Universal Label must start with the following byte sequence: 0x06, 0x0e, 0x2b, 0x34");
            }

            // set properties 
            SetCategoryAndRegistryDesignator();

            StructureDesignator = this[6];
            VersionNumber = this[7];
            ItemDesignator = new byte[] { this[8], this[9], this[10], this[11], this[12], this[13], this[14], this[15] };

            SMPTEInformation = SMPTERegisters.GetULDescription(this);
            Name = SMPTEInformation?.Name;

            if (this.IdentifiesPrivatelyRegisteredUL())
            {
                if (privateOrganizationsIdentifiers.TryGetValue(this[9], out string organization))
                {
                    Name = $"Privately Registered UL ({organization})";
                }
                else
                {
                    Name = $"Privately Registered UL";
                }
            }
        }

        public static bool HasValidULPrefix(params byte[] bytes)
        {
            return bytes.Length >= 4 &&
                bytes[0] == ValidULPrefix[0] &&
                bytes[1] == ValidULPrefix[1] &&
                bytes[2] == ValidULPrefix[2] &&
                bytes[3] == ValidULPrefix[3];
        }

        public bool IdentifiesLocalSet()
        {
            return CategoryDesignator == ULCategories.Groups &&
                    (RegistryDesignator == ULRegistries.LocalSet_1Byte_1Byte ||
                    RegistryDesignator == ULRegistries.LocalSet_1Byte_2Bytes ||
                    RegistryDesignator == ULRegistries.LocalSet_1Byte_4Bytes ||
                    RegistryDesignator == ULRegistries.LocalSet_1Byte_OIDBER ||

                    RegistryDesignator == ULRegistries.LocalSet_2Bytes_1Byte ||
                    RegistryDesignator == ULRegistries.LocalSet_2Bytes_2Bytes ||
                    RegistryDesignator == ULRegistries.LocalSet_2Bytes_4Bytes ||
                    RegistryDesignator == ULRegistries.LocalSet_2Bytes_OIDBER ||

                    RegistryDesignator == ULRegistries.LocalSet_4Bytes_1Byte ||
                    RegistryDesignator == ULRegistries.LocalSet_4Bytes_2Bytes ||
                    RegistryDesignator == ULRegistries.LocalSet_4Bytes_1Byte ||
                    RegistryDesignator == ULRegistries.LocalSet_4Bytes_OIDBER ||

                    RegistryDesignator == ULRegistries.LocalSet_BER_1Byte ||
                    RegistryDesignator == ULRegistries.LocalSet_BER_2Bytes ||
                    RegistryDesignator == ULRegistries.LocalSet_BER_4Bytes ||
                    RegistryDesignator == ULRegistries.LocalSet_BER_OIDBER);
        }


        public bool IdentifiesLocalSet_2BytesLength2BytesTag()
        {
            return CategoryDesignator == ULCategories.Groups &&
                RegistryDesignator == ULRegistries.LocalSet_2Bytes_2Bytes;
        }

        public bool IdentifiesEssence()
        {
            return CategoryDesignator == ULCategories.Elements &&
                RegistryDesignator == ULRegistries.EssenceDictionaries;
        }

        public bool IdentifiesPrivatelyRegisteredUL()
        {
            return this[8] == 0x0e;
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool shortFormat)
        {
            if (shortFormat)
            {
                if (Name == null)
                {
                    return base.ToString();
                }
                else
                {
                    return $"{Name}";
                }
            }
            else
            {
                return $"{Name ?? "Unknown"} - {base.ToString()}";
            }
        }

        private void SetCategoryAndRegistryDesignator()
        {
            switch (this[4])
            {
                case 0x01:
                    CategoryDesignator = ULCategories.Elements;
                    switch (this[5])
                    {
                        case 0x01:
                            RegistryDesignator = ULRegistries.MetadataDictionaries;
                            break;
                        case 0x02:
                            RegistryDesignator = ULRegistries.EssenceDictionaries;
                            break;
                        case 0x03:
                            RegistryDesignator = ULRegistries.ControlDictionaries;
                            break;
                        case 0x04:
                            RegistryDesignator = ULRegistries.TypesDictionaries;
                            break;
                        default:
                            RegistryDesignator = null;
                            break;
                    }
                    break;

                case 0x02:
                    CategoryDesignator = ULCategories.Groups;
                    switch (this[5])
                    {
                        // Universal sets

                        case 0x01:
                            RegistryDesignator = ULRegistries.UniversalSet;
                            break;

                        // Global sets

                        case 0x02:
                            RegistryDesignator = ULRegistries.GlobalSet_BER;
                            break;
                        case 0x22:
                            RegistryDesignator = ULRegistries.GlobalSet_1Byte;
                            break;
                        case 0x42:
                            RegistryDesignator = ULRegistries.GlobalSet_2Bytes;
                            break;
                        case 0x62:
                            RegistryDesignator = ULRegistries.GlobalSet_4Bytes;
                            break;

                        // Local sets

                        case 0x03:
                            RegistryDesignator = ULRegistries.LocalSet_BER_1Byte;
                            break;
                        case 0x0b:
                            RegistryDesignator = ULRegistries.LocalSet_BER_OIDBER;
                            break;
                        case 0x13:
                            RegistryDesignator = ULRegistries.LocalSet_BER_2Bytes;
                            break;
                        case 0x1b:
                            RegistryDesignator = ULRegistries.LocalSet_BER_4Bytes;
                            break;
                        case 0x23:
                            RegistryDesignator = ULRegistries.LocalSet_1Byte_1Byte;
                            break;
                        case 0x2b:
                            RegistryDesignator = ULRegistries.LocalSet_1Byte_OIDBER;
                            break;
                        case 0x33:
                            RegistryDesignator = ULRegistries.LocalSet_1Byte_2Bytes;
                            break;
                        case 0x3b:
                            RegistryDesignator = ULRegistries.LocalSet_1Byte_4Bytes;
                            break;
                        case 0x43:
                            RegistryDesignator = ULRegistries.LocalSet_2Bytes_1Byte;
                            break;
                        case 0x4b:
                            RegistryDesignator = ULRegistries.LocalSet_2Bytes_OIDBER;
                            break;
                        case 0x53:
                            RegistryDesignator = ULRegistries.LocalSet_2Bytes_2Bytes;
                            break;
                        case 0x5b:
                            RegistryDesignator = ULRegistries.LocalSet_2Bytes_4Bytes;
                            break;
                        case 0x63:
                            RegistryDesignator = ULRegistries.LocalSet_4Bytes_1Byte;
                            break;
                        case 0x6b:
                            RegistryDesignator = ULRegistries.LocalSet_4Bytes_OIDBER;
                            break;
                        case 0x73:
                            RegistryDesignator = ULRegistries.LocalSet_4Bytes_2Bytes;
                            break;
                        case 0x7b:
                            RegistryDesignator = ULRegistries.LocalSet_4Bytes_4Bytes;
                            break;

                        // Variable length packs

                        case 0x04:
                            RegistryDesignator = ULRegistries.VariableLengthPacks_BER;
                            break;
                        case 0x24:
                            RegistryDesignator = ULRegistries.VariableLengthPacks_1Byte;
                            break;
                        case 0x44:
                            RegistryDesignator = ULRegistries.VariableLengthPacks_2Bytes;
                            break;
                        case 0x64:
                            RegistryDesignator = ULRegistries.VariableLengthPacks_4Bytes;
                            break;

                        // DefinedLengthPacks

                        case 0x05:
                            RegistryDesignator = ULRegistries.DefinedLengthPacks;
                            break;

                        case 0x06:
                            RegistryDesignator = ULRegistries.Reserved;
                            break;

                        default:
                            RegistryDesignator = null;
                            break;
                    }
                    break;

                case 0x03:
                    CategoryDesignator = ULCategories.ContainersAndWrappers;
                    switch (this[5])
                    {
                        case 0x01:
                            RegistryDesignator = ULRegistries.SimpleWrappersAndContainers;
                            break;
                        case 0x02:
                            RegistryDesignator = ULRegistries.ComplexWrappersAndContainers;
                            break;
                    }
                    break;

                case 0x04:
                    CategoryDesignator = ULCategories.Labels;
                    break;

                case 0x05:
                    CategoryDesignator = ULCategories.RegisteredPrivate;
                    break;

                case byte b when b >= 0x06 && b <= 0x7e:
                    CategoryDesignator = ULCategories.Reserved;
                    break;

                default:
                    CategoryDesignator = null;
                    break;
            }
        }


    }

}
