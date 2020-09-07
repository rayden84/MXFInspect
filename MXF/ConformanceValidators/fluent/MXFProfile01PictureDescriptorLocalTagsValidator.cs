using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FluentValidation;
using FluentValidation.Internal;

namespace Myriadbits.MXF.ConformanceValidators
{
    public class MXFProfile01PictureDescriptorLocalTagsValidator : AbstractValidator<IEnumerable<MXFLocalTag>>
    {
        // expected values

        private const uint bitRate = 50 * 1000 * 1000;
        private const ushort maxGOPSize = 12;
        private const ushort maxBPicCount = 2;
        private const byte codedContentScanningKind = 2;
        private const byte identicalGOPIndicator1 = 0;
        private const byte identicalGOPIndicator2 = 1;
        private const byte bPicFlag1 = 0;
        private const byte bPicFlag2 = 1;
        private const byte profileAndLevel = 130;
        private const byte singleSequenceFlag1 = 0;
        private const byte singleSequenceFlag2 = 1;
        private const byte closedGOPIndicator1 = 0;
        private const byte closedGOPIndicator2 = 1;
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

        public MXFProfile01PictureDescriptorLocalTagsValidator(MXFCDCIPictureEssenceDescriptor desc)
        {
            // localtags that must be inspected

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

            // rules 
            var a = this.CreateDescriptor();

            CascadeMode = CascadeMode.StopOnFirstFailure;

            // Bit Rate[158W], 50_000_000
            RuleFor(tags => GetTagValue(bitRate_Tag))
                .NotNull()
                .WithName("Bit Rate [158W]")
                .WithState(tags => bitRate.ToString("N0"))
                .Equal(bitRate);

            // Identical GOP Indicator [224W], [False, True]
            RuleFor(tags => GetTagValue(identicalGOP_Tag))
                            .NotNull()
                            .WithName("Identical GOP Indicator [224W]")
                            .WithState(tags => identicalGOPIndicator1 + " || " + identicalGOPIndicator2)
                            .Must(v => v.Equals(identicalGOPIndicator1) || v.Equals(identicalGOPIndicator2));

            // Maximum GOP Size [46W], (ushort)12
            RuleFor(tags => GetTagValue(maxGOPSize_Tag))
                .NotNull()
                .WithName("Maximum GOP Size [46W]")
                .WithState(tags => maxGOPSize.ToString())
                .Equal(maxGOPSize);

            // Maximum B Picture Count [46W], (ushort)2
            RuleFor(tags => GetTagValue(maxBPicCount_Tag))
                            .NotNull()
                            .WithName("Maximum B Picture Count [46W]")
                            .WithState(tags => maxBPicCount.ToString())
                            .Equal(maxBPicCount);

            // Constant B Picture Flag [225W], [False, True]
            RuleFor(tags => GetTagValue(constBPicFlag_Tag))
                            .NotNull()
                            .WithName("Constant B Picture Flag [225W]")
                            .WithState(tags => bPicFlag1 + " || " + bPicFlag2)
                            .Must(v => v.Equals(bPicFlag1) || v.Equals(bPicFlag2));

            // Coded Content Scanning Kind [226W], 0x800E, (byte)2
            RuleFor(tags => GetTagValue(contentScanningKind_Tag))
                .NotNull()
                .WithName("Coded Content Scanning Kind [226W]")
                .WithState(tags => codedContentScanningKind.ToString())
                .Equal(codedContentScanningKind);

            // Profile And Level [35W], (byte)130 
            RuleFor(tags => GetTagValue(profileAndLevel_Tag))
                            .NotNull()
                            .WithName("Profile And Level [35W]")
                            .WithState(tags => profileAndLevel.ToString())
                            .Equal(profileAndLevel);

            // Single Sequence Flag [243W], [False, True]
            RuleFor(tags => GetTagValue(singleSequenceFlag_Tag))
                            .NotNull()
                            .WithName("Single Sequence Flag [243W]")
                            .WithState(tags => singleSequenceFlag1 + " || " + singleSequenceFlag2)
                            .Must(v => v.Equals(singleSequenceFlag1) || v.Equals(singleSequenceFlag2));

            // Closed GOP Indicator [45W], [False, True]
            RuleFor(tags => GetTagValue(closedGOP_Tag))
                .NotNull()
                .WithName("Closed GOP Indicator [45W]")
                .WithState(tags => closedGOPIndicator1 + " || " + closedGOPIndicator2)
                .Must(v => v.Equals(closedGOPIndicator1) || v.Equals(closedGOPIndicator2));

            // Low Delay Indicator [227W], (byte)0
            RuleFor(tags => GetTagValue(lowDelay_Tag))
                .NotNull()
                .WithName("Low Delay Indicator [227W]")
                //.WithState(tags => lowDelayIndicator.ToString())
                .Equal(lowDelayIndicator);
        }

        private MXFLocalTag GetLocalTagByAliasUID(MXFCDCIPictureEssenceDescriptor desc, MXFKey key)
        {
            var localTagKey = GetLocalTagKeyByAliasUID(desc, key);
            if (key != null)
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
