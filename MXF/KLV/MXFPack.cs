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
using System.Linq;

namespace Myriadbits.MXF
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MXFPack : KLVTriplet
    {
        public override UL Key { get; }

        // TODO should the length be overriden?
        //public override KLVLength Key { get; }

        [Browsable(false)]
        public MXFPartition Partition { get; set; }
        
        public MXFPack(UL key, KLVLength length, long offset) : base(key, length, offset)
        {
            // needed since it is overriden
            Key = key;
        }

        public override string ToString()
        {
            return $"{Key.SMPTEInformation?.Name} - PackLength: {this.TotalLength}";
        }
    }
}
