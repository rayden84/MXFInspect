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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Myriadbits.MXF
{
    public class MXFProfile01PictureDescriptorValidator : MXFValidator
    {
        private MXFCDCIPictureEssenceDescriptor Descriptor { get; set; }
        private IEnumerable<MXFLocalTag> LocalTags { get; set; }
        private MXFValidationResult Result { get; set; } = new MXFValidationResult("Picture Descriptor Test");

        public override void OnExecuteTest(ref List<MXFValidationResult> results)
        {
            this.Task = "PictureDescriptor";
            var descriptors = this.File.FlatList.OfType<MXFCDCIPictureEssenceDescriptor>();
            results.Add(Result);

            if (descriptors.Count() > 1)
            {
                Result.AddError("More than one picture descriptor found!");
            }
            else if (descriptors.Count() == 0)
            {
                Result.AddError("No picture descriptor found");
            }
            else
            {
                Descriptor = descriptors.First();
                LocalTags = Descriptor.Children.OfType<MXFLocalTag>();

                CheckPictureDescriptor();

                if (LocalTags.Any())
                {
                    CheckLocalTags();
                }
                else
                {
                    Result.AddError("No local tags found.");
                }

                if (Result.Any(i => i.State == MXFValidationState.Error))
                {
                    Result.SetError("At least one rule failed.");
                }
                else
                {
                    Result.SetSuccess("Picture descriptor is conformant to profile.");
                }

            }
        }

        private void CheckPictureDescriptor()
        {
            // TODO: Picture Essence Coding[105W]

            CheckPictureProperty("Aspect Ratio[69W]", Descriptor.AspectRatio, new MXFRational { Num = 16, Den = 9 });
            CheckPictureProperty("Sample Rate [42W]", Descriptor.SampleRate, new MXFRational { Num = 25, Den = 1 });

            // TODO: Container Duration[40W]

            CheckPictureProperty("Field Dominance[41W]", Descriptor.FieldDominance, (byte?)1);
            CheckPictureProperty("Signal Standard [162W]", Descriptor.SignalStandard, (byte?)4);
            CheckPictureProperty("Frame Layout [214W]", Descriptor.FrameLayout, (byte?)1);
            CheckPictureProperty("Display Width [43W]", Descriptor.DisplayWidth, (uint?)1920);
            CheckPictureProperty("Display Height [43W]", Descriptor.DisplayHeight, (uint?)540);
            CheckPictureProperty("Sample Width [163W]", Descriptor.SampledWidth, (uint?)1920);
            CheckPictureProperty("Sample Height [163W]", Descriptor.SampledHeight, (uint?)540);
            CheckPictureProperty("Stored Width [70W]", Descriptor.StoredWidth, (uint?)1920);
            CheckPictureProperty("Stored Height [70W]", Descriptor.StoredHeight, (uint?)544);
            CheckPictureProperty("Stored F2 Offset [161W]", Descriptor.StoredF2Offset, 0);
            CheckPictureProperty("Sampled X Offset [161W]", Descriptor.SampledXOffset, 0);
            CheckPictureProperty("Sampled Y Offset [161W]", Descriptor.SampledYOffset, 0);
            CheckPictureProperty("Display X Offset [161W]", Descriptor.DisplayXOffset, 0);
            CheckPictureProperty("Display Y Offset [161W]", Descriptor.DisplayYOffset, 0);
            CheckPictureProperty("Display F2 Offset [161W]", Descriptor.DisplayF2Offset, 0);

            // TODO
            //CheckValue("Active Format Descriptor [1W]", Descriptor.A, 1920);

            // TODO: why {2,4...}?
            CheckPictureProperty("Video Line Map [159W]", Descriptor.VideoLineMap, new int[] { 2, 4, 21, 584 });

            // TODO CheckValue("Transfer Characteristic / Capture Gamma [215W]", Descriptor.SignalStandard, 4);

            CheckPictureProperty("Image Start Offset [216W]", Descriptor.ImageStartOffset, (uint?)0);
            CheckPictureProperty("Image End Offset [237W]", Descriptor.ImageEndOffset, (uint?)0);
            CheckPictureProperty("Color Siting [217W]", Descriptor.ColorSiting, (uint?)0);
            CheckPictureProperty("Padding Bits [218W]", Descriptor.PaddingBits, (short?)0);
            CheckPictureProperty("Black Ref Level [219W]", Descriptor.BlackRefLevel, (uint?)16);
            CheckPictureProperty("White Ref Level [220W]", Descriptor.WhiteRefLevel, (uint?)235);
            CheckPictureProperty("Color Range [221W]", Descriptor.ColorRange, (uint?)225);
            CheckPictureProperty("Horizontal Subsampling [34W]", Descriptor.HorizontalSubsampling, (uint?)2);
            CheckPictureProperty("Vertical Subsampling [34W]", Descriptor.VerticalSubsampling, (uint?)1);
            CheckPictureProperty("Component Depth [32W]", Descriptor.ComponentDepth, (uint?)8);
        }

        private void CheckLocalTags()
        {
            CheckLocalTagValue("Bit Rate [158W]", 0x8015, (uint)50 * 1000 * 1000);
            // TODO: not really strict
            //AssertEquals("Identical GOP Indicator [224W]",0x8011, true or false);
            CheckLocalTagValue("Maximum GOP Size [46W]", 0x8012, (ushort)12);
            CheckLocalTagValue("Maximum B Picture Count [46W]", 0x8013, (ushort)2);
            // TODO: not really strict
            //AssertEquals("Constant B Picture Flag [225W]", GetLocalTag(pd, 0x800D)?.Value,[False, True]);
            CheckLocalTagValue("Coded Content Scanning Kind [226W]", 0x800E, (byte)2);
            CheckLocalTagValue("Profile And Level [35W]", 0x8014, (byte)130);

            // TODO: not really strict
            //AssertEquals("Single Sequence Flag [243W]", p0x8014,[False, True]);
            //AssertEquals("Closed GOP Indicator [45W]", p0x8014,[False, True]);
            CheckLocalTagValue("Low Delay Indicator [227W]", 0x800F, (byte)0);
        }

        private void CheckPictureProperty<T>(string ruleName, T actual, T expected)
        {
            var str = string.Format("Rule: {0} Expected: {1} Actual: {2}", ruleName, expected.ToString(), actual.ToString());
            if (AssertAllEqual<T>(actual, expected))
            {
                Result.AddSuccess(str);
            }
            else
            {
                Result.AddError(str);
            }
        }


        private void Check<T>(string ruleName, Func<T, T, bool> outcome)
        {
            var str = string.Format("Rule: {0} Expected: {1} Actual: {2}", ruleName, expected.ToString(), actual.ToString());
            if (outcome.Invoke())
            {
                Result.AddSuccess(str);
            }
            else
            {
                Result.AddError(str);
            }
        }
        //private bool AssertEqual<T>(T actual, T expected)
        //{
        //    if(actual is Array)
        //    {
        //        return false;
        //        //var a = Array.Fr(actual 
        //        //return AssertAllEqual((actual), expected);
        //    }
        //    else
        //    {
        //        return actual.Equals(expected);
        //    }
        //}


        private bool AssertAllEqual<T>(T[] actualValues, T[] expectedValues)
        {
            if (ReferenceEquals(actualValues, expectedValues))
                return true;

            if (actualValues == null || expectedValues == null)
                return false;

            if (actualValues.Length != expectedValues.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < actualValues.Length; i++)
            {
                if (!comparer.Equals(actualValues[i], expectedValues[i])) return false;
            }
            return true;
        }

        private void CheckMultipleValue<T>(string ruleName, T actual, T[] expected)
        {
            string multiValues = "{" + string.Join(",", expected.ToString()) + "}";
            var str = string.Format("Rule: {0} Expected: {1} Actual: {2}", ruleName, multiValues, actual.ToString());
            if (expected.Contains(actual))
            {
                Result.AddSuccess(str);
            }
            else
            {
                Result.AddError(str);
            }
        }

        private void CheckLocalTagValue<T>(string ruleName, ushort tag, T expectedValue)
        {
            var tagObj = LocalTags.First(t => t.Tag == tag);

            if (tagObj != null)
            {
                try
                {
                    var tagValue = (T)tagObj.Value;
                    CheckPictureProperty(ruleName, tagValue, expectedValue);
                }
                catch (Exception)
                {
                    Result.AddError(string.Format("Unable to interpret value for localtag {0:x}", tag));
                }
            }
            else
            {
                Result.AddError(string.Format("LocalTag {0:x} not found!", tag));
            }
        }
    }
}
