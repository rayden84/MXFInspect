using Myriadbits.MXF.ConformanceValidators.validation_Framework;
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

        public MXFProfile01PictureDescriptorValidator1(MXFCDCIPictureEssenceDescriptor descriptor)
        {
            // Essence Container Label(Video essence mapping)[212W]
            AddRule(desc => desc.EssenceContainer)
                .WithName("Essence Container Label (Video essence mapping) [212W]")
                .EqualTo(ConformanceValidationKeys.GC_FrameWrapped_MPEG_VideoStream0_SID_Key);


            // TODO Picture Element Key[165W]
            // 060e2b34.01020101.0d010301.15010500(= MXF Generic Container Version 1 SMPTE 381M MPEG Frame - Wrapped Picture Essence)
            //AddRule(desc => desc).EqualTo(ConformanceValidationKeys.GC_FrameWrapped_MPEG_VideoStream0_SID_Key);

            // Picture Essence Coding [105W]
            AddRule(desc => desc.PictureEssenceCoding)
                .WithName("Picture Essence Coding [105W]")
                .EqualTo(ConformanceValidationKeys.MPEG2_422P_HL_Long_GOP_Coding_Key);


            // Aspect Ratio [69W]
            AddRule(desc => desc.AspectRatio)
                .WithName("Aspect Ratio [69W]")
                .EqualTo(new MXFRational { Num = 16, Den = 9 });


            // Sample Rate [42W]
            AddRule(desc => desc.SampleRate)
                .WithName("Sample Rate [42W]")
                .EqualTo(new MXFRational { Num = 25, Den = 1 });

            // TODO: Container Duration[40W]
            // Shall be present and identical with audio Container Duration. 
            // If the partition status is incomplete, the value may be absent.

            // Field Dominance [41W]
            AddRule(desc => desc.FieldDominance)
                .WithName("Field Dominance [41W]")
                .EqualTo((byte?)1);


            // Signal Standard [162W] 
            AddRule(desc => desc.SignalStandard)
                .WithName("Signal Standard [162W]")
                .EqualTo((byte?)4);


            // Frame Layout [214W]
            AddRule(desc => desc.FrameLayout)
                .WithName("Frame Layout [214W]")
                .EqualTo((byte?)1);


            // Display Width [43W]
            AddRule(desc => desc.DisplayWidth)
                .WithName("Display Width [43W]")
                .EqualTo((uint?)1920);


            // Display Height [43W]
            AddRule(desc => desc.DisplayHeight)
                .WithName("Display Height [43W]")
                .EqualTo((uint?)540);


            // Sample Width [163W]
            AddRule(desc => desc.SampledWidth)
                .WithName("Sample Width [163W]")
                .EqualTo((uint?)1920);


            // Sample Height [163W]
            AddRule(desc => desc.SampledHeight)
                .WithName("Sample Height [163W]")
                .EqualTo((uint?)540);


            // Stored Width [70W]
            AddRule(desc => desc.StoredWidth)
                .WithName("Stored Width [70W]")
                .EqualTo((uint?)1920);


            // Stored Height [70W]
            AddRule(desc => desc.StoredHeight)
                .WithName("Stored Height [70W]")
                .EqualTo((uint?)544);


            // Stored F2 Offset [161W]
            AddRule(desc => desc.StoredF2Offset)
                .WithName("Stored F2 Offset [161W]")
                .EqualTo(0);


            // Sampled X Offset [161W]
            AddRule(desc => desc.SampledXOffset)
                .WithName("Sampled X Offset [161W]")
                .EqualTo(0);


            // Sampled Y Offset [161W]
            AddRule(desc => desc.SampledYOffset)
                .WithName("Sampled Y Offset [161W]")
                .EqualTo(0);


            // Display X Offset [161W]
            AddRule(desc => desc.DisplayXOffset)
                .WithName("Display X Offset [161W]")
                .EqualTo(0);


            // Display Y Offset [161W]
            AddRule(desc => desc.DisplayYOffset)
                .WithName("Display Y Offset [161W]")
                .EqualTo(0);


            // Display F2 Offset [161W]
            AddRule(desc => desc.DisplayF2Offset)
                .WithName("Display F2 Offset [161W]")
                .EqualTo(0);


            // TODO: optimize values as list of possible values
            // Active Format Descriptor [1W]
            AddRule(desc => desc.ActiveFormatDescriptor)
                .WithName("Active Format Descriptor [1W]")
                .EqualToOneOf(validActiveFormatDescriptors);

            // Video Line Map [159W]
            AddRule(desc => desc.VideoLineMap)
                .WithName("Video Line Map [159W]")
                .EqualTo(validVideoLineMap, new EnumerableComparer<int>());

            //.MustSatisfy(desc => IsValidVideoLineMap(desc.VideoLineMap))

            //.WithState(afd => ValidVideoLineMapToString());


            // Transfer Characteristic / Capture Gamma [215W]
            AddRule(desc => desc.TransferCharacteristics)
                .WithName("Transfer Characteristic / Capture Gamma [215W]")
                .EqualTo(ConformanceValidationKeys.ITU_R_BT_709_Transfer_Characteristic_Key);


            // Image Start Offset [216W]
            AddRule(desc => desc.ImageStartOffset)
                .WithName("Image Start Offset [216W]")
                .EqualTo((uint?)0);


            // Image End Offset [237W]
            AddRule(desc => desc.ImageEndOffset)
                .WithName("Image End Offset [237W]")
                .EqualTo((uint?)0);


            // Color Siting [217W]
            AddRule(desc => desc.ColorSiting)
                .WithName("Color Siting [217W]")
                .EqualTo((byte?)0);


            // Padding Bits [218W]
            AddRule(desc => desc.PaddingBits)
                .WithName("Padding Bits [218W]")
                .EqualTo((short?)0);


            // Black Ref Level [219W]
            AddRule(desc => desc.BlackRefLevel)
                .WithName("Black Ref Level [219W]")
                .EqualTo((uint?)16);


            // White Ref Level [220W]
            AddRule(desc => desc.WhiteRefLevel)
                .WithName("White Ref Level [220W]")
                .EqualTo((uint?)235);


            // Color Range [221W]
            AddRule(desc => desc.ColorRange)
                .WithName("Color Range [221W]")
                .EqualTo((uint?)225);


            // Horizontal Subsampling [34W]
            AddRule(desc => desc.HorizontalSubsampling)
                .WithName("Horizontal Subsampling [34W]")
                .EqualTo((uint?)2);


            // Vertical Subsampling [34W]
            AddRule(desc => desc.VerticalSubsampling)
                .WithName("Vertical Subsampling [34W]")
                .EqualTo((uint?)1);


            // Component Depth [32W]
            AddRule(desc => desc.ComponentDepth)
                .WithName("Component Depth [32W]")
                .EqualTo((uint?)8);
        }
    }
}
