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

using Myriadbits.MXF.Identifiers;
using Myriadbits.MXF.Identifiers.UL;
using Myriadbits.MXF.Metadata;
using Myriadbits.MXF.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Myriadbits.MXF
{

    /// <summary>
    /// Create the correct MXF (sub) object 
    /// </summary>
    public class MXFPackFactory
    {
        private static readonly IReadOnlyDictionary<ByteArray, Type> dict = new Dictionary<ByteArray, Type>(new KeyPartialMatchComparer())
        {
            #region Main Elements
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x05, 0x01, 0x01, 0x0d, 0x01, 0x02, 0x01, 0x01, 0x02), typeof(MXFPartition)} ,               // Header partition
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x05, 0x01, 0x01, 0x0d, 0x01, 0x02, 0x01, 0x01, 0x03), typeof(MXFPartition)},                 // Body partition
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x05, 0x01, 0x01, 0x0d, 0x01, 0x02, 0x01, 0x01, 0x04), typeof(MXFPartition)},                 // Footer partition

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x05, 0x01, 0x01, 0x0d, 0x01, 0x02, 0x01, 0x01, 0x05, 0x01, 0x00), typeof(MXFPrimerPack)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x05, 0x01, 0x01, 0x0d, 0x01, 0x02, 0x01, 0x01, 0x11, 0x01, 0x00), typeof(MXFRIP)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x05, 0x01, 0x01, 0x0d, 0x01, 0x03, 0x01, 0x04), typeof(MXFSystemItem)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x03, 0x01, 0x14), typeof(MXFSystemItem)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x01, 0x02, 0x01, 0x01, 0x0d, 0x01, 0x03, 0x01, 0x05), typeof(MXFEssenceElement)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x01, 0x02, 0x01, 0x01, 0x0d, 0x01, 0x03, 0x01, 0x06), typeof(MXFEssenceElement)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x01, 0x02, 0x01, 0x01, 0x0d, 0x01, 0x03, 0x01, 0x07), typeof(MXFEssenceElement)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x01, 0x02, 0x01, 0x01, 0x0d, 0x01, 0x03, 0x01, 0x15), typeof(MXFEssenceElement)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x01, 0x02, 0x01, 0x01, 0x0d, 0x01, 0x03, 0x01, 0x16), typeof(MXFEssenceElement)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x01, 0x02, 0x01, 0x01, 0x0d, 0x01, 0x03, 0x01, 0x18), typeof(MXFEssenceElement)},

            // TODO cannot be found in SMPTE official registers ???
            // closest one: OrganizationallyRegisteredAsPrivate 	http://www.smpte-ra.org/reg/400/2012/14 	urn:smpte:ul:060e2b34.04010101.0e000000.00000000
            // "SonyMpeg4ExtraData ???, 
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x04, 0x01, 0x01, 0x01, 0x0e, 0x06, 0x06, 0x02, 0x02, 0x01, 0x00, 0x00), typeof(MXFPack)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x01, 0x02, 0x01, 0x01, 0x0d, 0x01, 0x03, 0x01, 0x17), typeof(MXFANCFrameElement)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x43, 0x01, 0x01, 0x0d, 0x01, 0x03, 0x01, 0x04, 0x01), typeof(MXFPackageMetaData)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x63, 0x01, 0x01, 0x0d, 0x01, 0x03, 0x01, 0x04, 0x01), typeof(MXFPackageMetaData)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x02, 0x01, 0x01, 0x10, 0x01, 0x00), typeof(MXFIndexTableSegment)},

            #endregion

            #region Main Elements

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x04, 0x01, 0x02, 0x02, 0x00, 0x00), typeof(MXFCryptographicContext)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x2F, 0x00), typeof(MXFPreface)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x30, 0x00), typeof(MXFIdentification)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x18, 0x00), typeof(MXFContentStorage)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x23, 0x00), typeof(MXFEssenceContainerData)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x19, 0x00), typeof(MXFControlPoint)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x34, 0x00), typeof(MXFGenericPackage)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x36, 0x00), typeof(MXFMaterialPackage)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x37, 0x00), typeof(MXFSourcePackage)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x38, 0x00), typeof(MXFGenericTrack)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x39, 0x00), typeof(MXFEventTrack)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x3A, 0x00), typeof(MXFStaticTrack)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x3B, 0x00), typeof(MXFTimelineTrack)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x03, 0x00), typeof(MXFSegment)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x05, 0x00), typeof(MXFEssenceGroup)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x06, 0x00), typeof(MXFEvent)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x07, 0x00), typeof(MXFGPITrigger)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x0a, 0x00), typeof(MXFOperationGroup)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x0b, 0x00), typeof(MXFNestedScope)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x0c, 0x00), typeof(MXFPulldown)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x0d, 0x00), typeof(MXFScopeReference)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x0e, 0x00), typeof(MXFSelector)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x0F, 0x00), typeof(MXFSequence)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x10, 0x00), typeof(MXFSourceReference)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x11, 0x00), typeof(MXFSourceClip)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x12, 0x00), typeof(MXFTextClip)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x13, 0x00), typeof(MXFHTMLClip)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x14, 0x00), typeof(MXFTimecodeComponent)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x15, 0x00), typeof(MXFTimecodeStream)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x16, 0x00), typeof(MXFTimecodeStream12M)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x17, 0x00), typeof(MXFTransition)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x1a, 0x00), typeof(MXFDefinitionObject)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x1b, 0x00), typeof(MXFDataDefinition)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x1c, 0x00), typeof(MXFOperationDefinition)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x1d, 0x00), typeof(MXFParameterDefinition)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x1f, 0x00), typeof(MXFCodecDefinition)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x7d, 0x00), typeof(MXFOpDefinition)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x7e, 0x00), typeof(MXFCompressionDefinition)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x20, 0x00), typeof(MXFContainerDefinition)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x21, 0x00), typeof(MXFInterpolationDefinition)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x22, 0x00), typeof(MXFDictionary)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x4c, 0x00), typeof(MXFTaggedValueDefinition)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x4d, 0x00), typeof(MXFKLVDataDefinition)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x41, 0x00), typeof(MXFDescriptiveMarker)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x57, 0x00), typeof(MXFDynamicMarker)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x58, 0x00), typeof(MXFDynamicClip)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x60, 0x00), typeof(MXFDescriptiveClip)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x08, 0x00), typeof(MXFCommentMarker)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x09, 0x00), typeof(MXFFiller)}, // Filler
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x01, 0x01, 0x01, 0x02, 0x03, 0x01, 0x02, 0x10, 0x01, 0x00, 0x00, 0x00), typeof(MXFFiller)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x01, 0x01, 0x01, 0x01, 0x03, 0x01, 0x02, 0x10, 0x01, 0x00, 0x00, 0x00), typeof(MXFFiller)}, // Old filler
            #endregion

            #region Descriptors
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x25, 0x00), typeof(MXFFileDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x26, 0x00), typeof(MXFAIFCDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x27, 0x00), typeof(MXFGenericPictureEssenceDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x28, 0x00), typeof(MXFCDCIPictureEssenceDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x2a, 0x00), typeof(MXFHTMLDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x2b, 0x00), typeof(MXFTIFFDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x2c, 0x00), typeof(MXFWAVEDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x51, 0x00), typeof(MXFMPEGPictureEssenceDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x52, 0x00), typeof(MXFParsedTextDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x5d, 0x00), typeof(MXFDCPCMSoundDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x5e, 0x00), typeof(MXFMPEGAudioDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x5f, 0x00), typeof(MXFVC1VideoDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x29, 0x00), typeof(MXFRGBAPictureEssenceDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x42, 0x00), typeof(MXFGenericSoundEssenceDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x43, 0x00), typeof(MXFGenericDataEssenceDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x44, 0x00), typeof(MXFMultipleDescriptor)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x31, 0x00), typeof(MXFLocator)}, // Locator
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x32, 0x00), typeof(MXFNetworkLocator)}, // Network Locator
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x33, 0x00), typeof(MXFTextLocator)}, // Text Locator
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x61, 0x00), typeof(MXFGenericDescriptor)}, // Application Plug-in object
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x62, 0x00), typeof(MXFGenericDescriptor)}, // Application Referenced object
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x69, 0x00), typeof(MXFTIFFPictureEssenceDescriptor)},


            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x47, 0x00), typeof(MXFAES3PCMDescriptor)}, // Descriptor: AES3
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x48, 0x00), typeof(MXFWAVEPCMDescriptor)}, // Descriptor: Wave
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x5B, 0x00), typeof(MXFGenericDataEssenceDescriptor)}, // Descriptor: VBI Data Descriptor, SMPTE 436 - 7.3
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x5C, 0x00), typeof(MXFGenericDataEssenceDescriptor)}, // Descriptor: ANC Data Descriptor, SMPTE 436 - 7.3

            // DCTimedTextDescriptor per SMPTE ST 429-5
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x64, 0x00), typeof(MXFDCTimedTextDescriptor)},
            #endregion

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x3c, 0x00), typeof(MXFParameter)},

            #region Sub-Descriptors

            // ACESPictureSubDescriptor SMPTE ST 2067-50
            // urn:smpte:ul:060e2b34.027f0101.0d010101.01017900
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x79, 0x00), typeof(MXFACESPictureSubDescriptor)},

            // TargetFrameSubDescriptor SMPTE ST 2067-50
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x7a, 0x00), typeof(MXFTargetFrameSubDescriptor)},

            // JPEG 2000 SubDescriptor per SMPTE ST 422 
            // urn:smpte:ul:060e2b34.027f0101.0d010101.01015a00
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x5a, 0x00), typeof(MXFJPEG2000SubDescriptor)},


            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x59, 0x00), typeof(MXFSubDescriptor)},


            // MCA Label SubDescriptors per SMPTE ST 377-4
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x6a, 0x00), typeof(MXFMCALabelSubDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x6b, 0x00), typeof(MXFAudioChannelLabelSubDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x6c, 0x00), typeof(MXFSoundfieldGroupLabelSubDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x6d, 0x00), typeof(MXFGroupOfSoundfieldGroupsLabelSubDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x67, 0x00), typeof(MXFContainerConstraintsSubDescriptor)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x6e, 0x00), typeof(MXFAVCSubDescriptor)},

            #endregion

            #region DescriptiveObjects

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x04, 0x01, 0x04, 0x02, 0x01, 0x00), typeof(MXFGenericStreamTextBasedSet)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x04, 0x01, 0x04, 0x03, 0x01, 0x00), typeof(MXFTextBasedObject)},

            // DescriptiveObject	
            // urn:smpte:ul:060e2b34.027f0101.0d010400.00000000
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x04, 0x01, 0x00, 0x00, 0x00, 0x00), typeof(MXFDescriptiveObject)},

            // Thesaurus
            // urn:smpte:ul:060e2b34.027f0101.0d010401.017f1200
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x04, 0x01, 0x01, 0x7f, 0x12, 0x00), typeof(MXFThesaurus)},

            // Contact 
            // urn:smpte:ul:060e2b34.027f0101.0d010401.017f1a00 
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x04, 0x01, 0x01, 0x7f, 0x1a, 0x00), typeof(MXFContact)},

            // Location
            // urn:smpte:ul:060e2b34.027f0101.0d010401.011a0400
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x04, 0x01, 0x01, 0x1a, 0x04, 0x00), typeof(MXFLocation)},

            // ContactsList
            // urn:smpte:ul:060e2b34.027f0101.0d010401.01190100
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x04, 0x01, 0x01, 0x19, 0x01, 0x00), typeof(MXFContactsList)},

            // Address
            // urn:smpte:ul:060e2b34.027f0101.0d010401.011b0100
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x04, 0x01, 0x01, 0x1b, 0x01, 0x00), typeof(MXFAddress)},

            #endregion

            #region DescriptiveFrameworks

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x04, 0x01, 0x04, 0x01, 0x01, 0x00), typeof(MXFTextBasedFramework)},

            // ProductionFramework 
            // urn:smpte:ul:060e2b34.027f0101.0d010401.01010100
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x04, 0x01, 0x01, 0x01, 0x01, 0x00), typeof(MXFProductionFramework)},

            // ProductionClipFramework 
            // urn:smpte:ul: 060e2b34.027f0101.0d010401.017f0200
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x04, 0x01, 0x01, 0x7f, 0x02, 0x00), typeof(MXFProductionClipFramework)},

            // DMS1Framework 
            // urn:smpte:ul:060e2b34.027f0101.0d010401.017f0100
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x04, 0x01, 0x01, 0x7f, 0x01, 0x00), typeof(MXFDMS1Framework)},
            #endregion

            #region Uncategorized
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x01, 0x01, 0x3f, 0x00), typeof(MXFTaggedValue)},

            // XMLDocumentText_Indirect
            // urn:smpte:ul:060e2b34.01010105.03010220.01000000
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x01, 0x01, 0x01, 0x05, 0x03, 0x01, 0x02, 0x20, 0x01, 0x00, 0x00, 0x00), typeof(MXFXMLDocumentText_Indirect)},

            // ItemValue_ISO7
            //urn:smpte:ul:060e2b34.01010105.0301020a.02000000
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x01, 0x01, 0x01, 0x05, 0x03, 0x01, 0x02, 0x0a, 0x02, 0x00, 0x00, 0x00), typeof(MXFItemValue_ISO7)},

            // LensUnitAcquisitionMetadata 
            // urn:smpte:ul:060e2b34.027f0101.0c020101.01010000
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0c, 0x02, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00), typeof(MXFLensUnitAquisitionMetadata)},

            // CameraUnitAcquisitionMetadata 
            // urn:smpte:ul:060e2b34.027f0101.0c020101.02010000
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0c, 0x02, 0x01, 0x01, 0x02, 0x01, 0x00, 0x00), typeof(MXFCameraUnitAquisitionMetadata)},

            // MetaDictionary - urn:smpte:ul:060e2b34.027f0101.0d010101.02250000
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x25, 0x00, 0x00), typeof(MXFMetaDictionary)},

            #endregion

            #region MetaDefinition
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x24, 0x00, 0x00), typeof(MXFMetaDefinition)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x03, 0x00, 0x00), typeof(MXFTypeDefinition)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x02, 0x00, 0x00), typeof(MXFPropertyDefinition)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x01, 0x00, 0x00), typeof(MXFClassDefinition)},

            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x04, 0x00, 0x00), typeof(MXFTypeDefinitionInteger)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x05, 0x00, 0x00), typeof(MXFTypeDefinitionStrongObjectReference)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x06, 0x00, 0x00), typeof(MXFTypeDefinitionWeakObjectReference)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x07, 0x00, 0x00), typeof(MXFTypeDefinitionEnumeration)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x08, 0x00, 0x00), typeof(MXFTypeDefinitionFixedArray)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x09, 0x00, 0x00), typeof(MXFTypeDefinitionVariableArray)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x0a, 0x00, 0x00), typeof(MXFTypeDefinitionSet)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x0b, 0x00, 0x00), typeof(MXFTypeDefinitionString)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x0c, 0x00, 0x00), typeof(MXFTypeDefinitionStream)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x0d, 0x00, 0x00), typeof(MXFTypeDefinitionRecord)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x0e, 0x00, 0x00), typeof(MXFTypeDefinitionRename)},

            //{new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x0f, 0x00, 0x00), typeof(MXFTypeDefinitionSet)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x20, 0x00, 0x00), typeof(MXFTypeDefinitionExtendibleEnumeration)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x21, 0x00, 0x00), typeof(MXFTypeDefinitionIndirect)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x22, 0x00, 0x00), typeof(MXFTypeDefinitionOpaque)},
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x02, 0x53, 0x01, 0x01, 0x0d, 0x01, 0x01, 0x01, 0x02, 0x23, 0x00, 0x00), typeof(MXFTypeDefinitionCharacter)},
            #endregion
        };


        /// <summary>
        /// Create a new MXF object based on the KLV key
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="currentPartition"></param>
        /// <returns></returns>
        public static MXFPack CreatePack(MXFPack pack, MXFReader reader)
        {

            if (dict.TryGetValue(pack.Key, out Type foundType))
            {
                return (MXFPack)Activator.CreateInstance(foundType, reader, pack);
            }
            else if (pack.Key.IdentifiesLocalSet_2BytesLength2BytesTag())
            {
                // TODO we need something that make the object that is not found in dict distinctable 
                // TODO this could be handled also with a partial UL, check if better approach
                return new MXFLocalSet(reader, pack);
            }

            return pack;
        }

        public static void SetDescriptionAttributesForAllTypes()
        {
            foreach(Type t in dict.Values)
            {
                SetDescriptionAttribute(t);
            }
        }

        private static void SetDescriptionAttribute(Type type)
        {
            // recursive calls up to the inheritance chain

            if (type.BaseType != null)
            {
                SetDescriptionAttribute(type.BaseType);
            }

            // iterate through all properties

            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(type))
            {
                // get the short key of the property, marked via the ULAttribute 

                var ul = prop.Attributes.OfType<ULElementAttribute>().FirstOrDefault()?.UL;
                var knownKeys = SMPTEULDictionary.GetEntries();

                if (ul != null)
                {
                    if (knownKeys.TryGetValue(ul, out var keyDescription))
                    {
                        prop.AddAttribute(new DescriptionAttribute(keyDescription.Definition));
                    }

                }
            }
        }

        // TODO should it be public or internal?
        internal class KeyPartialMatchComparer : IEqualityComparer<ByteArray>
        {
            // if the keys to compare are of the same category (meaning the same hash) compare
            // whether the byte sequence is equal
            public bool Equals(ByteArray x, ByteArray y)
            {
                return x.HasSameBeginning(y);
            }

            public int GetHashCode(ByteArray obj)
            {
                // hash only the first 12 bytes (prefix is 4 bytes + 5th byte = key category)
                return obj.ArrayLength >= 12 ? obj.GetHashCode(12) : obj.GetHashCode();
                //return obj.GetHashCode(obj.ArrayLength},
            }
        }
    }
}
