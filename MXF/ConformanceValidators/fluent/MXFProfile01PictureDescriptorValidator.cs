using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation;



namespace Myriadbits.MXF.ConformanceValidators
{
    public class MXFProfile01PictureDescriptorValidator : AbstractValidator<MXFCDCIPictureEssenceDescriptor>
    {
        private readonly IEnumerable<byte> validActiveFormatDescriptors = new List<byte>()
        {
            0b00000100,  // undefined, aspect ratio 16:9
            0b00100100,  // letterbox, aspect ratio 16:9
            0b01000100,  // full frame, aspect ratio 16:9
            0b01001100,  // pillarbox, aspect ratio 16:9
        };

        private readonly int[] validVideoLineMap = new int[] { 2, 4, 21, 584 };


        public MXFProfile01PictureDescriptorValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;


            // Essence Container Label(Video essence mapping)[212W]
            RuleFor(desc => desc.EssenceContainer)
                .Equal(ConformanceValidationKeys.GC_FrameWrapped_MPEG_VideoStream0_SID_Key)
                .WithName("Essence Container Label (Video essence mapping) [212W]");

            // TODO Picture Element Key[165W]
            // 060e2b34.01020101.0d010301.15010500(= MXF Generic Container Version 1 SMPTE 381M MPEG Frame - Wrapped Picture Essence)
            //RuleFor(desc => desc).Equal(ConformanceValidationKeys.GC_FrameWrapped_MPEG_VideoStream0_SID_Key);

            // Picture Essence Coding [105W]
            RuleFor(desc => desc.PictureEssenceCoding)
                .Equal(ConformanceValidationKeys.MPEG2_422P_HL_Long_GOP_Coding_Key)
                .WithName("Picture Essence Coding [105W]");

            // Aspect Ratio [69W]
            RuleFor(desc => desc.AspectRatio)
                .Equal(new MXFRational { Num = 16, Den = 9 })
                .WithName("Aspect Ratio [69W]");

            // Sample Rate [42W]
            RuleFor(desc => desc.SampleRate)
                .Equal(new MXFRational { Num = 25, Den = 1 })
                .WithName("Sample Rate [42W]");

            // TODO: Container Duration[40W]
            // Shall be present and identical with audio Container Duration. 
            // If the partition status is incomplete, the value may be absent.

            // Field Dominance [41W]
            RuleFor(desc => desc.FieldDominance)
                .Equal((byte?)1)
                .WithName("Field Dominance [41W]");

            // Signal Standard [162W] 
            RuleFor(desc => desc.SignalStandard)
                .Equal((byte?)4)
                .WithName("Signal Standard [162W]");

            // Frame Layout [214W]
            RuleFor(desc => desc.FrameLayout)
                .Equal((byte?)1)
                .WithName("Frame Layout [214W]");

            // Display Width [43W]
            RuleFor(desc => desc.DisplayWidth)
                .Equal((uint?)1920)
                .WithName("Display Width [43W]");

            // Display Height [43W]
            RuleFor(desc => desc.DisplayHeight)
                .Equal((uint?)540)
                .WithName("Display Height [43W]");

            // Sample Width [163W]
            RuleFor(desc => desc.SampledWidth)
                .Equal((uint?)1920)
                .WithName("Sample Width [163W]");

            // Sample Height [163W]
            RuleFor(desc => desc.SampledHeight)
                .Equal((uint?)540)
                .WithName("Sample Height [163W]");

            // Stored Width [70W]
            RuleFor(desc => desc.StoredWidth)
                .Equal((uint?)1920)
                .WithName("Stored Width [70W]");

            // Stored Height [70W]
            RuleFor(desc => desc.StoredHeight)
                .Equal((uint?)544)
                .WithName("Stored Height [70W]");

            // Stored F2 Offset [161W]
            RuleFor(desc => desc.StoredF2Offset)
                .Equal(0)
                .WithName("Stored F2 Offset [161W]");

            // Sampled X Offset [161W]
            RuleFor(desc => desc.SampledXOffset)
                .Equal(0)
                .WithName("Sampled X Offset [161W]");

            // Sampled Y Offset [161W]
            RuleFor(desc => desc.SampledYOffset)
                .Equal(0)
                .WithName("Sampled Y Offset [161W]");

            // Display X Offset [161W]
            RuleFor(desc => desc.DisplayXOffset)
                .Equal(0)
                .WithName("Display X Offset [161W]");

            // Display Y Offset [161W]
            RuleFor(desc => desc.DisplayYOffset)
                .Equal(0)
                .WithName("Display Y Offset [161W]");

            // Display F2 Offset [161W]
            RuleFor(desc => desc.DisplayF2Offset)
                .Equal(0)
                .WithName("Display F2 Offset [161W]");

            // TODO: optimize values as list of possible values
            // Active Format Descriptor [1W]
            RuleFor(desc => desc.ActiveFormatDescriptor)
                .Must(afd => IsValidActiveFormatDescriptor(afd))
                .WithName("Active Format Descriptor [1W]")
                .WithState(afd => ValidActiveFormatDescriptorsToString());

            // Video Line Map [159W]
            RuleFor(desc => desc.VideoLineMap)
                .Must(vlm => IsValidVideoLineMap(vlm))
                .WithName("Video Line Map [159W]")
                .WithState(afd => ValidVideoLineMapToString());


            // Transfer Characteristic / Capture Gamma [215W]
            RuleFor(desc => desc.TransferCharacteristics)
                .Equal(ConformanceValidationKeys.ITU_R_BT_709_Transfer_Characteristic_Key)
                .WithName("Transfer Characteristic / Capture Gamma [215W]");

            // Image Start Offset [216W]
            RuleFor(desc => desc.ImageStartOffset)
                .Equal((uint?)0)
                .WithName("Image Start Offset [216W]");

            // Image End Offset [237W]
            RuleFor(desc => desc.ImageEndOffset)
                .Equal((uint?)0)
                .WithName("Image End Offset [237W]");

            // Color Siting [217W]
            RuleFor(desc => desc.ColorSiting)
                .Equal((byte?)0)
                .WithName("Color Siting [217W]");

            // Padding Bits [218W]
            RuleFor(desc => desc.PaddingBits)
                .Equal((short?)0)
                .WithName("Padding Bits [218W]");

            // Black Ref Level [219W]
            RuleFor(desc => desc.BlackRefLevel)
                .Equal((uint?)16)
                .WithName("Black Ref Level [219W]");

            // White Ref Level [220W]
            RuleFor(desc => desc.WhiteRefLevel)
                .Equal((uint?)235)
                .WithName("White Ref Level [220W]");

            // Color Range [221W]
            RuleFor(desc => desc.ColorRange)
                .Equal((uint?)225)
                .WithName("Color Range [221W]");

            // Horizontal Subsampling [34W]
            RuleFor(desc => desc.HorizontalSubsampling)
                .Equal((uint?)2)
                .WithName("Horizontal Subsampling [34W]");

            // Vertical Subsampling [34W]
            RuleFor(desc => desc.VerticalSubsampling)
                .Equal((uint?)1)
                .WithName("Vertical Subsampling [34W]");

            // Component Depth [32W]
            RuleFor(desc => desc.ComponentDepth)
                .Equal((uint?)8)
                .WithName("Component Depth [32W]");

            // validate local tags
            RuleFor(desc => desc.Children.OfType<MXFLocalTag>())
                .SetValidator(desc => new MXFProfile01PictureDescriptorLocalTagsValidator(desc));
        }



        private string ValidActiveFormatDescriptorsToString()
        {
            return string.Join(" || ", validActiveFormatDescriptors);
        }

        private string ValidVideoLineMapToString()
        {
            return string.Join(" || ", validVideoLineMap);
        }

        public bool IsValidActiveFormatDescriptor(byte? formatDescriptor)
        {
            if (formatDescriptor != null)
            {
                return validActiveFormatDescriptors.Contains(formatDescriptor.Value);
            }
            else return false;

        }

        public bool IsValidVideoLineMap(int[] videoLineMap)
        {
            if (videoLineMap != null)
            {
                return videoLineMap.SequenceEqual(validVideoLineMap);
            }
            else return false;
        }
    }
}
