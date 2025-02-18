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
using System;
using Myriadbits.MXF.EssenceParser;
using Myriadbits.MXF.Identifiers;

namespace Myriadbits.MXF.EssenceParsers
{
    public class MXFEssenceParserFactory
    {
        private static readonly IReadOnlyDictionary<ByteArray, Type> dict = new Dictionary<ByteArray, Type>(new SMPTEEssenceRegisterComparer())
        {
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x01, 0x02, 0x01, 0x01, 0x0d, 0x01, 0x03, 0x01, 0x15, 0x7f, 0x17, 0x7f), typeof(MXFProResEssenceInfo) },
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x01, 0x02, 0x01, 0x01, 0x0d, 0x01, 0x03, 0x01, 0x15, 0x7f, 0x05, 0x7f), typeof(MXFMPEG2EssenceInfo) }
        };

        public static MXFEssenceInfoBase ParseEssence(MXFEssenceElement el)
        {
            if (dict.TryGetValue(el.Key, out Type foundType))
            {
                return (MXFEssenceInfoBase)Activator.CreateInstance(foundType, el);
            }
            return null;
        }
    };
}
