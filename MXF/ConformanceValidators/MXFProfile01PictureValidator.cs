using Myriadbits.MXF.ConformanceValidators.validation_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Myriadbits.MXF.ConformanceValidators
{
    public class MXFProfile01PictureValidator : Validator<IEnumerable<MXFLocalTag>>
    {
        // expected values

        private const uint bitRate = 50 * 1000 * 1000;
        private const ushort maxGOPSize = 12;
        private const ushort maxBPicCount = 2;
        private const byte codedContentScanningKind = 2;

        private readonly IEnumerable<byte> validIdenticalGOPIndicators = new List<byte> { 0, 1 };
        private readonly IEnumerable<byte> validBPictureFlags = new List<byte> { 0, 1 };
        private const byte profileAndLevel = 130;     
        private readonly IEnumerable<byte> validSingleSequenceFlags = new List<byte> { 0, 1 };      
        private readonly IEnumerable<byte> validClosedGOPIndicators = new List<byte> { 0, 1 };
        private const byte lowDelayIndicator = 0;

        // keys for local tags in Picture Descriptor

        private static readonly int[] firstKey = new int[] { 0x06, 0x0e, 0x2b, 0x34, 0x01, 0x01, 0x01, 0x05 };

        private readonly MXFKey BitRate_Key = new MXFKey(firstKey, new int[] { 0x04, 0x01, 0x06, 0x02, 0x01, 0x0b, 0x00, 0x00 });
        private readonly MXFKey IdenticalGOPIndicator_Key = new MXFKey(firstKey, new int[] { 0x04, 0x01, 0x06, 0x02, 0x01, 0x07, 0x00, 0x00 });
        private readonly MXFKey MaxGOPSize_Key = new MXFKey(firstKey, new int[] { 0x04, 0x01, 0x06, 0x02, 0x01, 0x08, 0x00, 0x00 });
        private readonly MXFKey MaxBPictureCount_Key = new MXFKey(firstKey, new int[] { 0x04, 0x01, 0x06, 0x02, 0x01, 0x09, 0x00, 0x00 });
        private readonly MXFKey ConstantBPictureFlag_Key = new MXFKey(firstKey, new int[] { 0x04, 0x01, 0x06, 0x02, 0x01, 0x03, 0x00, 0x00 });
        private readonly MXFKey ContentScanningKind_Key = new MXFKey(firstKey, new int[] { 0x04, 0x01, 0x06, 0x02, 0x01, 0x04, 0x00, 0x00 });
        private readonly MXFKey ProfileAndLevel_Key = new MXFKey(firstKey, new int[] { 0x04, 0x01, 0x06, 0x02, 0x01, 0x0a, 0x00, 0x00 });
        private readonly MXFKey SingleSequenceFlag_Key = new MXFKey(firstKey, new int[] { 0x04, 0x01, 0x06, 0x02, 0x01, 0x02, 0x00, 0x00 });
        private readonly MXFKey ClosedGOP_Key = new MXFKey(firstKey, new int[] { 0x04, 0x01, 0x06, 0x02, 0x01, 0x06, 0x00, 0x00 });
        private readonly MXFKey LowDelay_Key = new MXFKey(firstKey, new int[] { 0x04, 0x01, 0x06, 0x02, 0x01, 0x05, 0x00, 0x00 });

        public MXFProfile01PictureValidator(MXFCDCIPictureEssenceDescriptor desc)
        {
            MXFLocalTag bitRate_Tag = GetLocalTagByAliasUID(desc, BitRate_Key);
            MXFLocalTag identicalGOP_Tag = GetLocalTagByAliasUID(desc, IdenticalGOPIndicator_Key);
            MXFLocalTag maxGOPSize_Tag = GetLocalTagByAliasUID(desc, MaxGOPSize_Key);
            MXFLocalTag maxBPicCount_Tag = GetLocalTagByAliasUID(desc, MaxBPictureCount_Key);
            MXFLocalTag constBPicFlag_Tag = GetLocalTagByAliasUID(desc, ConstantBPictureFlag_Key);
            MXFLocalTag contentScanningKind_Tag = GetLocalTagByAliasUID(desc, ContentScanningKind_Key);
            MXFLocalTag profileAndLevel_Tag = GetLocalTagByAliasUID(desc, ProfileAndLevel_Key);
            MXFLocalTag singleSequenceFlag_Tag = GetLocalTagByAliasUID(desc, SingleSequenceFlag_Key);
            MXFLocalTag closedGOP_Tag = GetLocalTagByAliasUID(desc, ClosedGOP_Key);
            MXFLocalTag lowDelay_Tag = GetLocalTagByAliasUID(desc, LowDelay_Key);


            // Bit Rate[158W], 50_000_000
            AddRule(tags => GetTagValue(bitRate_Tag))
                .WithName("Bit Rate [158W]")
                .EqualTo(bitRate);


            // Identical GOP Indicator [224W], [False, True]
            AddRule(tags => GetTagValue(identicalGOP_Tag))
                 .WithName("Identical GOP Indicator [224W]")
                 .EqualToOneOf(validIdenticalGOPIndicators.Cast<object>());

            // Maximum GOP Size [46W], (ushort)12
            AddRule(tags => GetTagValue(maxGOPSize_Tag))
                .WithName("Maximum GOP Size [46W]")
                .EqualTo(maxGOPSize);

            // Maximum B Picture Count [46W], (ushort)2
            AddRule(tags => GetTagValue(maxBPicCount_Tag))
                            .WithName("Maximum B Picture Count [46W]")
                            .EqualTo(maxBPicCount);

            // Constant B Picture Flag [225W], [False, True]
            AddRule(tags => GetTagValue(constBPicFlag_Tag))
                            .WithName("Constant B Picture Flag [225W]")
                            .EqualToOneOf(validBPictureFlags.Cast<object>());

            // Coded Content Scanning Kind [226W], 0x800E, (byte)2
            AddRule(tags => GetTagValue(contentScanningKind_Tag))
                .WithName("Coded Content Scanning Kind [226W]")
                .EqualTo(codedContentScanningKind);

            // Profile And Level [35W], (byte)130 
            AddRule(tags => GetTagValue(profileAndLevel_Tag))
                            .WithName("Profile And Level [35W]")
                            .EqualTo(profileAndLevel);

            // Single Sequence Flag [243W], [False, True]
            AddRule(tags => GetTagValue(singleSequenceFlag_Tag))
                            .WithName("Single Sequence Flag [243W]")
                            .EqualToOneOf(validSingleSequenceFlags.Cast<object>());

            // Closed GOP Indicator [45W], [False, True]
            AddRule(tags => GetTagValue(closedGOP_Tag))
                .WithName("Closed GOP Indicator [45W]")
                .EqualToOneOf(validClosedGOPIndicators.Cast<object>());

            // Low Delay Indicator [227W], (byte)0
            AddRule(tags => GetTagValue(lowDelay_Tag))
                .WithName("Low Delay Indicator [227W]")
                .EqualTo(lowDelayIndicator);
        }


        private MXFLocalTag GetLocalTagByAliasUID(MXFCDCIPictureEssenceDescriptor desc, MXFKey key)
        {
            var localTagKey = GetLocalTagKeyByAliasUID(desc, key);
            if (localTagKey != null)
            {
                return desc.Children.OfType<MXFLocalTag>().FirstOrDefault(t => t.Tag == localTagKey);
            }
            else return null;
        }

        private ushort? GetLocalTagKeyByAliasUID(MXFCDCIPictureEssenceDescriptor desc, MXFKey key)
        {
            var file = desc.TopParent as MXFFile;
            if (file != null)
            {
                return file.FlatList
                    .OfType<MXFEntryPrimer>()?
                    .FirstOrDefault(e => e.AliasUID.Key == key)?
                    .LocalTag;
            }
            else
            {
                return null;
            }
        }

        private object GetTagValue(MXFLocalTag tag)
        {
            return tag?.Value;
        }
    }
}
