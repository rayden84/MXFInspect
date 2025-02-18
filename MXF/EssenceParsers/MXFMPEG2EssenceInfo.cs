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

using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System;
using Myriadbits.MXF.KLV;
using System.Collections.Generic;
using System.Threading;

namespace Myriadbits.MXF.EssenceParser
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MXFMPEG2EssenceInfo : MXFEssenceInfoBase
    {
        public static readonly ImmutableArray<byte> StartCodePrefix = ImmutableArray.Create<byte>(0x00, 0x00, 0x01);

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MPEGSequenceHeader Sequence { get; private set; }
        public List<MPEGExtension> Extensions { get; private set; } = new List<MPEGExtension>();

        public MXFMPEG2EssenceInfo(MXFEssenceElement el) : base(el)
        {
            using (reader)
            {
                while (reader.Position + 4 <= reader.BaseStream.Length)
                {

                    ReadStartCode();
                }
            }
        }

        private void ReadStartCode()
        {


            if (SeekToBytePattern(out long _, StartCodePrefix))
            {
                byte[] startCodeBytes = reader.ReadBytes(4);
                switch (startCodeBytes[3])
                {
                    case 0x00:      //Picture
                        break;
                    case >= 0x01 and <= 0xAF: //slice
                        break;
                    case 0xB0:    //reserved
                        break;
                    case 0xB1:    //reserved
                        break;
                    case 0xB2:    //user data
                        break;
                    case 0xB3:    //Sequence header
                        Sequence = new MPEGSequenceHeader(reader);
                        break;
                    case 0xB4:    //sequence error
                        break;
                    case 0xB5:    //extension
                        ReadExtension();
                        //Extensions.Add(new MPEGExtension(reader));
                        break;
                    case 0xB6:    //reserved
                        break;
                    case 0xB7:    //sequence end
                        break;
                    case 0xB8:    //Group of Pictures


                    default:
                        // should not happen
                        break;
                }
            }
            else
            {
                // no more to read
            }
        }

        // TODO bring this method up to the level of BinaryReader
        private bool SeekToBytePattern(out long newOffset, ImmutableArray<byte> bytePattern, long seekThresholdInBytes = 0, CancellationToken ct = default)
        {
            int foundBytes = 0;
            int bytesRead = 0;

            // TODO consider Boyer-Moore search algorithm
            while (!reader.EOF && (seekThresholdInBytes == 0 || bytesRead <= seekThresholdInBytes))
            {
                if (reader.ReadByte() == bytePattern[foundBytes])
                {
                    foundBytes++;

                    if (foundBytes == bytePattern.Length)
                    {
                        // pattern found, reposition to pattern beginning
                        reader.Seek(reader.Position - bytePattern.Length);
                        newOffset = reader.Position;
                        return true;
                    }
                }
                else
                {
                    foundBytes = 0;
                }

                ct.ThrowIfCancellationRequested();
                bytesRead++;
            }

            // TODO what does the caller have to do in this case?
            newOffset = reader.Position;
            return false;
        }

        private void ReadExtension()
        {
            int extension_identifier = (reader.ReadByte() & 0b1111_0000) >> 4;
            reader.Seek(reader.Position - 1);

            switch (extension_identifier)
            {
                case 0b0000: //reserved
                    break;
                case 0b0001: //Sequence Extension ID
                    Extensions.Add(new SequenceExtension(reader));
                    break;
                case 0b0010: //Sequence Display Extension ID
                    Extensions.Add(new SequenceDisplayExtension(reader));
                    break;
                case 0b0011: //Quant Matrix Extension ID
                    break;
                case 0b0100: //reserved
                    break;
                case 0b0101: //Sequence Scalable Extension ID
                    break;
                case 0b0110: //reserved
                    break;
                case 0b0111: //Picture Display Extension ID
                    break;
                case 0b1000: //Picture Coding Extension ID
                    break;
                case 0b1001: //Picture Spatial Scalable Extension ID
                    break;
                case 0b1010: //Picture Temporal Scalable Extension ID
                    break;
                case 0b1011: //reserved
                    break;
                case >= 0b1100: //reserved...
                default:
                    break;
            }
        }

        public override string ToString()
        {
            return $"MPEG2";
        }

        public class MPEGSequenceHeader
        {
            public int HorizontalSize { get; }
            public int VerticalSize { get; }
            [TypeConverter(typeof(EnumDescriptionConverter))]
            public MPEGAspectRatio AspectRatio { get; }
            [TypeConverter(typeof(EnumDescriptionConverter))]
            public MPEGFrameRate FrameRate { get; }
            public long Bitrate { get; }
            public int VBV_Buffersize { get; }
            public bool ConstrainedParametersFlag { get; }
            public bool load_non_intra_quantiser_matrix { get; }
            public bool load_intra_quantiser_matrix { get; }

            public MPEGSequenceHeader(KLVStreamReader reader)
            {
                // variable length
                var bytes = reader.ReadBytes(4);
                HorizontalSize = (bytes[0] << 4) | (bytes[1] & 0b1111_0000);
                VerticalSize = ((bytes[1] & 0b0000_1111) << 8) | bytes[2];
                int aspectratio = bytes[3] & 0b1111_0000 >> 4;
                int framerate = bytes[3] & 0b0000_1111;

                if (aspectratio >= (byte)MPEGAspectRatio.Reserved)
                {
                    AspectRatio = MPEGAspectRatio.Reserved;
                }
                else
                {
                    AspectRatio = (MPEGAspectRatio)aspectratio;
                }

                if (framerate >= (byte)MPEGFrameRate.Reserved)
                {
                    FrameRate = MPEGFrameRate.Reserved;
                }
                else
                {
                    FrameRate = (MPEGFrameRate)framerate;
                }

                bytes = reader.ReadBytes(4);

                Bitrate = bytes[0] << 12 | bytes[1] << 4 | (bytes[2] & 0b1100_0000);
                VBV_Buffersize = (bytes[2] & 0b0001_1111) << 6 | bytes[3] & 0b1111_1000;
                ConstrainedParametersFlag = ((bytes[3] & 0b0000_0100) >> 2) != 0;
                load_intra_quantiser_matrix = ((bytes[3] & 0b0000_0010) >> 1) != 0;

                // skip matrix
                if (load_intra_quantiser_matrix == true)
                {
                    bytes = reader.ReadBytes(64);
                    load_non_intra_quantiser_matrix = (bytes[63] & 0b0000_0001) != 0;
                }
                else
                {
                    load_non_intra_quantiser_matrix = (bytes[3] & 0b0000_0001) != 0;
                }

                if (load_non_intra_quantiser_matrix == true)
                {
                    bytes = reader.ReadBytes(64);
                }
            }

        }

        public class MPEGExtension
        {

            public MPEGExtension(KLVStreamReader reader)
            {

            }
        }

        public class SequenceExtension : MPEGExtension
        {

            public SequenceExtension(KLVStreamReader reader) : base(reader)
            {
                // fixed length of 6 bytes
                var bytes = reader.ReadBytes(2);
                var profile_and_level = (bytes[1] & 0b0000_1111) << 4 | (bytes[0] & 0b111_0000);

                reader.ReadBytes(4);
            }
        }

        public class SequenceDisplayExtension : MPEGExtension
        {
            public SequenceDisplayExtension(KLVStreamReader reader) : base(reader)
            {
                // variable length: either 5 or 8 bytes
                var bytes = reader.ReadByte();
                var videoformat = (MPEGVideoFormat)(bytes & 0b0000_1110 >> 1);
                bool color_flag = (bytes & 0b0000_0001) != 0;
                if (color_flag == true)
                {
                    reader.ReadBytes(3);
                }
                reader.ReadBytes(4);
            }
        }

        #region types
        public enum MPEGAspectRatio
        {
            [Description("Forbidden")]
            Forbidden = 0b0000,
            [Description("Square")]
            Square = 0b0001,
            [Description("4/3")]
            AspectRatio_4_3 = 0b0010,
            [Description("16/9")]
            AspectRatio_16_9 = 0b0011,
            [Description("2,21:1")]
            AspectRatio_2_21_1 = 0b0100,
            [Description("Reserved")]
            Reserved = 0b0101
        }

        public enum MPEGFrameRate
        {
            [Description("Forbidden")]
            Forbidden = 0b0000,
            [Description("24.000÷1001 (23,976)")]
            FrameRate_24000_1001 = 0b0001,
            [Description("24")]
            FrameRate_24 = 0b0010,
            [Description("25")]
            FrameRate_25 = 0b0011,
            [Description("30.000÷1001 (29,97)")]
            FrameRate_30000_1001 = 0b0100,
            [Description("30")]
            FrameRate_30 = 0b0101,
            [Description("50")]
            FrameRate_50 = 0b0110,
            [Description("60.000÷1001 (59,94)")]
            FrameRate_60000_1001 = 0b0111,
            [Description("60")]
            FrameRate_60 = 0b1000,
            [Description("Reserved")]
            Reserved = 0b1001
        }

        public enum MPEGVideoFormat
        {
            Component = 0b000,
            PAL = 0b001,
            NTSC = 0b010,
            SECAM = 0b011,
            MAC = 0b100,
            UnspecifiedVideoFormat = 0b101,
            Reserved0 = 0b110,
            Reserved1 = 0b111,
        }

        #endregion
    }


}


