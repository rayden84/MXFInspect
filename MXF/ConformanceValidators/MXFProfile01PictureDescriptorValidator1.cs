using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Myriadbits.MXF.ConformanceValidators
{
    public class MXFProfile01PictureDescriptorValidator1 : Validator<MXFCDCIPictureEssenceDescriptor>
    {
        private readonly IEnumerable<byte?> validActiveFormatDescriptors = new List<byte?>()
        {
            0b00000100,  // undefined, aspect ratio 16:9
            0b00100100,  // letterbox, aspect ratio 16:9
            0b01000100,  // full frame, aspect ratio 16:9
            0b01001100,  // pillarbox, aspect ratio 16:9
        };

        private readonly int[] validVideoLineMap = new int[] { 2, 4, 21, 584 };

        public MXFProfile01PictureDescriptorValidator1()
        {
            // Essence Container Label(Video essence mapping)[212W]
            AddRule(desc => desc.EssenceContainer)
                .EqualTo(ConformanceValidationKeys.GC_FrameWrapped_MPEG_VideoStream0_SID_Key)
                .WithName("Essence Container Label (Video essence mapping) [212W]");

            // TODO Picture Element Key[165W]
            // 060e2b34.01020101.0d010301.15010500(= MXF Generic Container Version 1 SMPTE 381M MPEG Frame - Wrapped Picture Essence)
            //AddRule(desc => desc).EqualTo(ConformanceValidationKeys.GC_FrameWrapped_MPEG_VideoStream0_SID_Key);

            // Picture Essence Coding [105W]
            AddRule(desc => desc.PictureEssenceCoding)
                .EqualTo(ConformanceValidationKeys.MPEG2_422P_HL_Long_GOP_Coding_Key)
                .WithName("Picture Essence Coding [105W]");

            // Aspect Ratio [69W]
            AddRule(desc => desc.AspectRatio)
                .EqualTo(new MXFRational { Num = 16, Den = 9 })
                .WithName("Aspect Ratio [69W]");

            // Sample Rate [42W]
            AddRule(desc => desc.SampleRate)
                .EqualTo(new MXFRational { Num = 25, Den = 1 })
                .WithName("Sample Rate [42W]");

            // TODO: Container Duration[40W]
            // Shall be present and identical with audio Container Duration. 
            // If the partition status is incomplete, the value may be absent.

            // Field Dominance [41W]
            AddRule(desc => desc.FieldDominance)
                .EqualTo((byte?)1)
                .WithName("Field Dominance [41W]");

            // Signal Standard [162W] 
            AddRule(desc => desc.SignalStandard)
                .EqualTo((byte?)4)
                .WithName("Signal Standard [162W]");

            // Frame Layout [214W]
            AddRule(desc => desc.FrameLayout)
                .EqualTo((byte?)1)
                .WithName("Frame Layout [214W]");

            // Display Width [43W]
            AddRule(desc => desc.DisplayWidth)
                .EqualTo((uint?)1920)
                .WithName("Display Width [43W]");

            // Display Height [43W]
            AddRule(desc => desc.DisplayHeight)
                .EqualTo((uint?)540)
                .WithName("Display Height [43W]");

            // Sample Width [163W]
            AddRule(desc => desc.SampledWidth)
                .EqualTo((uint?)1920)
                .WithName("Sample Width [163W]");

            // Sample Height [163W]
            AddRule(desc => desc.SampledHeight)
                .EqualTo((uint?)540)
                .WithName("Sample Height [163W]");

            // Stored Width [70W]
            AddRule(desc => desc.StoredWidth)
                .EqualTo((uint?)1920)
                .WithName("Stored Width [70W]");

            // Stored Height [70W]
            AddRule(desc => desc.StoredHeight)
                .EqualTo((uint?)544)
                .WithName("Stored Height [70W]");

            // Stored F2 Offset [161W]
            AddRule(desc => desc.StoredF2Offset)
                .EqualTo(0)
                .WithName("Stored F2 Offset [161W]");

            // Sampled X Offset [161W]
            AddRule(desc => desc.SampledXOffset)
                .EqualTo(0)
                .WithName("Sampled X Offset [161W]");

            // Sampled Y Offset [161W]
            AddRule(desc => desc.SampledYOffset)
                .EqualTo(0)
                .WithName("Sampled Y Offset [161W]");

            // Display X Offset [161W]
            AddRule(desc => desc.DisplayXOffset)
                .EqualTo(0)
                .WithName("Display X Offset [161W]");

            // Display Y Offset [161W]
            AddRule(desc => desc.DisplayYOffset)
                .EqualTo(0)
                .WithName("Display Y Offset [161W]");

            // Display F2 Offset [161W]
            AddRule(desc => desc.DisplayF2Offset)
                .EqualTo(0)
                .WithName("Display F2 Offset [161W]");

            // TODO: optimize values as list of possible values
            // Active Format Descriptor [1W]
            AddRule(desc => desc.ActiveFormatDescriptor)
                .EqualToOneOf(validActiveFormatDescriptors)
                .WithName("Active Format Descriptor [1W]");
            //.WithState(afd => ValidActiveFormatDescriptorsToString());

            // Video Line Map [159W]
            AddRule(desc => desc.VideoLineMap)
                .EqualTo(validVideoLineMap, new EnumerableComparer<int>())
                //.MustSatisfy(desc => IsValidVideoLineMap(desc.VideoLineMap))
                .WithName("Video Line Map [159W]");
                //.WithState(afd => ValidVideoLineMapToString());


            // Transfer Characteristic / Capture Gamma [215W]
            AddRule(desc => desc.TransferCharacteristics)
                .EqualTo(ConformanceValidationKeys.ITU_R_BT_709_Transfer_Characteristic_Key)
                .WithName("Transfer Characteristic / Capture Gamma [215W]");

            // Image Start Offset [216W]
            AddRule(desc => desc.ImageStartOffset)
                .EqualTo((uint?)0)
                .WithName("Image Start Offset [216W]");

            // Image End Offset [237W]
            AddRule(desc => desc.ImageEndOffset)
                .EqualTo((uint?)0)
                .WithName("Image End Offset [237W]");

            // Color Siting [217W]
            AddRule(desc => desc.ColorSiting)
                .EqualTo((byte?)0)
                .WithName("Color Siting [217W]");

            // Padding Bits [218W]
            AddRule(desc => desc.PaddingBits)
                .EqualTo((short?)0)
                .WithName("Padding Bits [218W]");

            // Black Ref Level [219W]
            AddRule(desc => desc.BlackRefLevel)
                .EqualTo((uint?)16)
                .WithName("Black Ref Level [219W]");

            // White Ref Level [220W]
            AddRule(desc => desc.WhiteRefLevel)
                .EqualTo((uint?)235)
                .WithName("White Ref Level [220W]");

            // Color Range [221W]
            AddRule(desc => desc.ColorRange)
                .EqualTo((uint?)225)
                .WithName("Color Range [221W]");

            // Horizontal Subsampling [34W]
            AddRule(desc => desc.HorizontalSubsampling)
                .EqualTo((uint?)2)
                .WithName("Horizontal Subsampling [34W]");

            // Vertical Subsampling [34W]
            AddRule(desc => desc.VerticalSubsampling)
                .EqualTo((uint?)1)
                .WithName("Vertical Subsampling [34W]");

            // Component Depth [32W]
            AddRule(desc => desc.ComponentDepth)
                .EqualTo((uint?)8)
                .WithName("Component Depth [32W]");

        }

        public bool IsValidActiveFormatDescriptor(byte? formatDescriptor)
        {
            if (formatDescriptor != null)
            {
                return validActiveFormatDescriptors.Contains(formatDescriptor.Value);
            }
            else return false;

        }
    }
}
