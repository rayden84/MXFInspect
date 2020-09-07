using FluentValidation;
using Myriadbits.MXF.ConformanceValidators.validation_Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Myriadbits.MXF.ConformanceValidators
{
    public class MXFProfile01AudioDescriptorValidator1 : Validator<MXFAES3AudioEssenceDescriptor>
    {
        private enum ChannelStatusMode
        {
            STANDARD,
            MINIMUM
        }

        private readonly IDictionary<ChannelStatusMode, byte[]> validChannelStatusModes = new Dictionary<ChannelStatusMode, byte[]>()
        {
            { ChannelStatusMode.STANDARD, new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x02 } }, // standard mode
            { ChannelStatusMode.MINIMUM , new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x01 } }  // minimum mode
        };

        private readonly IDictionary<ChannelStatusMode, byte[]> validChannelStatusDataForPCM = new Dictionary<ChannelStatusMode, byte[]>()
        {
            // standard mode, Professional Use, Linear PCM, No Emphasis, 48kHz Sampling, the CRCC value: 60
            { ChannelStatusMode.STANDARD,
                new byte[] {0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x18, 0x85, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60}
            },
            
            //  minimum mode, Professional Use, Linear PCM, No Emphasis, 48kHz Sampling
            { ChannelStatusMode.MINIMUM,
                new byte[] {0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x18, 0x85, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}
            }
        };

        private readonly byte[] validChannelStatusDataForDolby = new byte[]
        {
                // Professional Use, Non-PCM, No Emphasis, 48kHz Sampling, No indicated Channel Mode, 
                // No User Information indicated, Max. Audio Sample Word Length: 24 bit, Encoded Audio Sample Word Length: 
                // not indicated, CRCC value: 39
                 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x18, 0x83, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00,
                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x39
        };

        // PCM: 060e2b34.0401010A.04020201.01000000(= PCM)
        public readonly MXFKey PCM_SoundEssenceCoding_Key = new MXFKey(
                new int[] { 0x06, 0x0e, 0x2b, 0x34, 0x04, 0x01, 0x01, 0x0A },
                new int[] { 0x04, 0x02, 0x02, 0x01, 0x01, 0x00, 0x00, 0x00 });

        // Undefined: 060e2b34.04010101.04020201.7f000000 (= Uncompressed Sound Coding, Undefined Sound Coding) 
        public readonly MXFKey Undefined_SoundEssenceCoding_Key = new MXFKey(
            new int[] { 0x06, 0x0e, 0x2b, 0x34, 0x04, 0x01, 0x01, 0x01 },
            new int[] { 0x04, 0x02, 0x02, 0x01, 0x7f, 0x00, 0x00, 0x00 });

        // Dolby-E: 060e2b34.04010101.04020202.03021c00(= Dolby-E Compressed Audio)
        public readonly MXFKey DolbyE_SoundEssenceCoding_Key = new MXFKey(
            new int[] { 0x06, 0x0e, 0x2b, 0x34, 0x04, 0x01, 0x01, 0x01 },
            new int[] { 0x04, 0x02, 0x02, 0x02, 0x03, 0x02, 0x1c, 0x00 });

        // 060e2b34.04010101.0d010301.02060300(= MXF - GC Frame - wrapped AES3 audio data)
        public readonly MXFKey GC_FrameWrapped_AES3_AudioData_Key = new MXFKey(
            new int[] { 0x06, 0x0e, 0x2b, 0x34, 0x04, 0x01, 0x01, 0x01 },
            new int[] { 0x0d, 0x01, 0x03, 0x01, 0x02, 0x06, 0x03, 0x00 });



        public MXFProfile01AudioDescriptorValidator1()
        {
            // Essence Container Label [212W] (Audio essence mapping)
            // 060e2b34.04010101.0d010301.02060300(= MXF - GC Frame - wrapped AES3 audio data)
            AddRule(desc => desc.EssenceContainer)
                .WithName("Essence Container Label [212W] (Audio essence mapping)")
                .EqualTo(ConformanceValidationKeys.GC_FrameWrapped_AES3_AudioData_Key);

            // Sound Essence Coding [230W] / Sound Essence Compression
            AddRule(desc => desc.SoundEssenceCoding)
                .WithName("Sound Essence Coding [230W] / Sound Essence Compression")
                .EqualToOneOf(new[] {
                    PCM_SoundEssenceCoding_Key,
                    Undefined_SoundEssenceCoding_Key,
                    DolbyE_SoundEssenceCoding_Key
                    });

            // Sample Rate
            AddRule(desc => desc.SampleRate)
                .WithName("Sample Rate")
                .EqualTo(new MXFRational { Num = 48000, Den = 1 });

            // TODO: Container Duration[9W]

            // Audio sampling rate [13W]
            AddRule(desc => desc.AudioSamplingRate)
                .WithName("Audio sampling rate [13W]")
                .EqualTo(new MXFRational { Num = 48000, Den = 1 });

            // Locked/Unlocked [231W]
            AddRule(desc => desc.Locked)
                .WithName("Locked/Unlocked [231W]")
                .EqualTo(true);

            // TODO: Dial Norm, unclear specs
            // AddRule(desc => desc.DialNorm)

            // TODO: Audio Ref Level
            // AddRule(desc => desc.AudioRefLevel).NotNull()

            // Channel Count[164W]
            AddRule(desc => desc.ChannelCount)
                .WithName("Channel Count [164W]")
                .EqualTo((uint)1);


            // Quantization bits [3W]
            AddRule(desc => desc.QuantizationBits)
                .WithName("Quantization bits [3W]")
                .EqualTo((uint)24);

            // Block Align[234W]
            AddRule(desc => desc.BlockAlign)
                .WithName("Block Align [234W]")
                .EqualTo((ushort)3);

            // Average Bytes per Second(AvgBps) [235W]
            AddRule(desc => desc.AveragesBytesPerSecond)
                .WithName("Average Bytes per Second(AvgBps) [235W]")
                .EqualTo((uint)144 * 1000);

            // TODO: Channel Status Mode(Byte Pattern) [236W], unclear specs
            AddRule(desc => desc.ChannelStatusMode)
                .WithName("Channel Status Mode(Byte Pattern) [236W]")
                .EqualToOneOf(new byte[][]{
                    validChannelStatusModes[ChannelStatusMode.MINIMUM],
                    validChannelStatusModes[ChannelStatusMode.STANDARD]
                }, new EnumerableComparer<byte>());


            // Fixed Channel Status Data(for PCM Audio)[146W]
            // TODO: check if rule really checks what the specs demand
            AddRule(desc => desc.FixedChannelStatusData)
                .WithName("Channel Status Data(for PCM Audio) [146W]")
                .When(desc => !IsSoundEssenceDolbyCoded(desc.SoundEssenceCoding) &&
                              IsChannelSatusModeStandard(desc.ChannelStatusMode))
                .EqualTo(validChannelStatusDataForPCM[ChannelStatusMode.STANDARD], new EnumerableComparer<byte>());


            // Fixed Channel Status Data (for PCM Audio) [146W]
            // TODO: check if rule really checks what the specs demand
            AddRule(desc => desc.FixedChannelStatusData)
                .WithName("Channel Status Data (for PCM Audio)")
                .When(desc => !IsSoundEssenceDolbyCoded(desc.SoundEssenceCoding) &&
                              IsChannelSatusModeMinimum(desc.ChannelStatusMode))
                .EqualTo(validChannelStatusDataForPCM[ChannelStatusMode.MINIMUM], new EnumerableComparer<byte>());


            // Fixed Channel Status Data(for Dolby-E) [146W]
            AddRule(desc => desc.FixedChannelStatusData)
                .WithName("Fixed Channel Status Data(for Dolby-E) [146W]")
                .When(desc => IsSoundEssenceDolbyCoded(desc.SoundEssenceCoding))
                .EqualTo(validChannelStatusDataForDolby, new EnumerableComparer<byte>());

        }

        public bool IsSoundEssenceDolbyCoded(MXFKey coding)
        {
            return coding == ConformanceValidationKeys.DolbyE_SoundEssenceCoding_Key;
        }

        public bool IsChannelSatusModeStandard(byte[] channelStatusMode)
        {
            return channelStatusMode?.SequenceEqual(validChannelStatusModes[ChannelStatusMode.STANDARD]) ?? false;
        }

        public bool IsChannelSatusModeMinimum(byte[] channelStatusMode)
        {
            return channelStatusMode?.SequenceEqual(validChannelStatusModes[ChannelStatusMode.MINIMUM]) ?? false;
        }
    }
}
