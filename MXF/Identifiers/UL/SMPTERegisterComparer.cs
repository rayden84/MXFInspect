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

using System.Collections.Generic;

namespace Myriadbits.MXF.Identifiers
{
    public class SMPTERegisterComparer : IEqualityComparer<ByteArray>
    {

        public bool IgnoreVersion { get; } = true;

        public SMPTERegisterComparer(bool ignoreVeriosn)
        {
            IgnoreVersion = ignoreVeriosn;
        }

        public bool Equals(ByteArray x, ByteArray y)
        {
            //if (x.ArrayLength != y.ArrayLength)
            //{
            //    return false;
            //}

            for (int i = 0; i < x.ArrayLength; i++)
            {
                // from https://registry.smpte-ra.org/view/draft/docs/Register%20(Groups)/UL%20prefix/
                // Note that Byte 6 of every Group UL in the Groups Register is 0x7f.
                // This is a “wildcard” value that will not appear in implementations.
                // In implementations Byte 6 is replaced with a value that indicates the kind of KLV coding being used,
                // defined in SMPTE ST 336. So, for example, in Header Metadata in MXF files Byte 6 of most Group ULs is 0x53.
                // Bypass version byte too if set via property
                
                if (i == 5 || (i == UL.VERSION_NUMBER_INDEX && IgnoreVersion))
                {
                    continue;
                }
                else if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(ByteArray theBytes)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                for (int i = 0; i < theBytes.ArrayLength; i++)
                {
                    // from https://registry.smpte-ra.org/view/draft/docs/Register%20(Groups)/UL%20prefix/
                    // Note that Byte 6 of every Group UL in the Groups Register is 0x7f.
                    // This is a “wildcard” value that will not appear in implementations.
                    // In implementations Byte 6 is replaced with a value that indicates the kind of KLV coding being used,
                    // defined in SMPTE ST 336. So, for example, in Header Metadata in MXF files Byte 6 of most Group ULs is 0x53.
                    // Bypass version byte too if set via property

                    if (i == 5 || (i == UL.VERSION_NUMBER_INDEX && IgnoreVersion))
                    {
                        continue;
                    }

                    else
                    {
                        hash = (hash ^ theBytes[i]) * p;
                    }
                }

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }
    }
}
