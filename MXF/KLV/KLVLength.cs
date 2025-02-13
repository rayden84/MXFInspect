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
using System.ComponentModel;

namespace Myriadbits.MXF.KLV
{
    public class KLVLength : ByteArray, ILength
    {
        public enum LengthEncodings
        {
            OneByte = 1,
            TwoBytes = 2,
            FourBytes = 4,
        }

        /// <summary>
        /// Gets the length value of the L part in a KLV
        /// </summary>
        [Description("Value of the length part of the KLV triplet")]
        public long Value { get; }

        public LengthEncodings LengthEncoding { get; }

        public KLVLength(LengthEncodings lengthEncoding, params byte[] bytes) : base(bytes)
        {
            LengthEncoding = lengthEncoding;
            Value = bytes.ToLong();

            // TODO where do we check if each byte does not exceed 0x7F?
            if (bytes.Length != (int)lengthEncoding)
            {
                throw new ArgumentException($"Declared length encoding ({lengthEncoding}) does not correspond to given array length ({bytes.Length})");
            }
        }

        public override string ToString()
        {
            return $"{LengthEncoding}, ({Value})";
        }
    }
}
