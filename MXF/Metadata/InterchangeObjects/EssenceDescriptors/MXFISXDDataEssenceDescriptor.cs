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

using System.ComponentModel;
using Myriadbits.MXF.Identifiers;
using Myriadbits.MXF.KLV;

namespace Myriadbits.MXF
{

    [ULGroup("urn:smpte:ul:060e2b34.027f0105.0e090502.00000000")]
    public class MXFISXDDataEssenceDescriptor : MXFDataEssenceDescriptor
    {
        private const string CATEGORYNAME = "ISXDDataEssenceDescriptor";

        private readonly UL namespaceUri_key = new UL(0x06, 0x0e, 0x2b, 0x34, 0x01, 0x01, 0x01, 0x05, 0x0e, 0x09, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00);


        [Category(CATEGORYNAME)]
        [ULElement("urn:smpte:ul:060e2b34.01010105.0e090400.00000000")]
        public string NameSpaceURI { get; set; }

        /// <summary>
        /// Constructor, set the correct descriptor name
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="pack"></param>
        public MXFISXDDataEssenceDescriptor(MXFPack pack)
            : base(pack)
        {
        }

        /// <summary>
        /// Overridden method to process local tags
        /// </summary>
        /// <param name="localTag"></param>
        protected override bool ReadLocalTagValue(IKLVStreamReader reader, MXFLocalTag localTag)
        {
            if (localTag.AliasUID != null)
            {
                switch (localTag.AliasUID)
                {
                    case var _ when localTag.AliasUID == namespaceUri_key:
                        this.NameSpaceURI = reader.ReadUTF8String(localTag.Length.Value);
                        localTag.Value = this.NameSpaceURI;
                        return true;

                }
            }
            return base.ReadLocalTagValue(reader, localTag);
        }

    }
}
