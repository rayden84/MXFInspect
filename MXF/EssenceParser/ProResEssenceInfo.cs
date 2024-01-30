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

using Myriadbits.MXF.KLV;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Myriadbits.MXF.EssenceParser
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ProResEssenceInfo
    {
        public static readonly ImmutableArray<byte> ICPF = ImmutableArray.Create<byte>(0x69, 0x63, 0x70, 0x66);
        protected readonly BinaryReader reader;

        [Browsable(false)]
        public MXFEssenceElement EssenceElement { get; private set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Frame Frame { get; private set; }

        public int log2_desired_slice_size_in_mb { get; private set; }

        public ProResEssenceInfo(MXFEssenceElement el)
        {
            EssenceElement = el;

            SubStream ss = el.GetValueStream();
            using (reader = new KLVStreamReader(ss))
            {
                Frame = new Frame(reader);
            }
        }

    }

    public class Frame
    {
        protected readonly BinaryReader reader;

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Picture FirstPicture { get; private set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Picture SecondPicture { get; private set; }

        public uint frame_size { get; set; }
        public ushort frame_header_size { get; set; }
        public byte[] frame_identifier { get; set; }
        public byte bitstream_version { get; set; }
        public byte[] encoder_identifier { get; set; }
        public ushort horizontal_size { get; set; }
        public ushort vertical_size { get; set; }
        public ProResAspectRatio? aspect_ratio_information { get; set; }
        public ProResChromaFormat? chroma_format { get; set; }
        public ProResInterlaceMode? interlace_mode { get; set; }

        [TypeConverter(typeof(EnumDescriptionConverter))]
        public ProResFrameRate? frame_rate_code { get; set; }
        public byte color_primaries { get; set; }
        public byte transfer_characteristic { get; set; }
        public byte matrix_coefficients { get; set; }
        public int alpha_channel_type { get; set; }
        public bool load_luma_quantization_matrix { get; set; }
        public bool load_chroma_quantization_matrix { get; set; }
        public long stuffing_size { get; private set; }

        public byte[,] chroma_quantization_matrix;

        public byte[,] luma_quantization_matrix;

        public Frame(BinaryReader _reader)
        {
            reader = _reader;
            frame_size = reader.ReadUInt32();
            frame_identifier = reader.ReadBytes(4); // should be 0x69637066 = ICPF

            if (frame_identifier.SequenceEqual(ProResEssenceInfo.ICPF))
            {
                ParseFrameHeader();

                // save reader pos
                long readerPos = reader.BaseStream.Position;

                FirstPicture = new Picture(reader, this, TemporalOrder.First);
                if (interlace_mode == ProResInterlaceMode.Interlaced_TFF || interlace_mode == ProResInterlaceMode.Interlaced_BFF)
                {
                    // position reader right after first picture
                    reader.BaseStream.Seek(readerPos + FirstPicture.picture_size, SeekOrigin.Begin);
                    SecondPicture = new Picture(reader, this, TemporalOrder.Second);
                }
                stuffing_size = CalculateStuffingSize();

                if (stuffing_size > 0)
                {
                    ParseStuffing(stuffing_size);
                }
            }
        }

        public override string ToString()
        {
            return $"{vertical_size} × {horizontal_size} {interlace_mode}";
        }

        private void ParseFrameHeader()
        {
            frame_header_size = reader.ReadUInt16();
            reader.ReadByte(); // skip one byte (reserved byte)
            bitstream_version = reader.ReadByte();
            encoder_identifier = reader.ReadBytes(4);
            horizontal_size = reader.ReadUInt16();
            vertical_size = reader.ReadUInt16();

            byte currByte = reader.ReadByte();
            chroma_format = (ProResChromaFormat)((currByte & 0b1100_0000) >> 6);
            interlace_mode = (ProResInterlaceMode)((currByte & 0b0000_1100) >> 2);

            currByte = reader.ReadByte();
            aspect_ratio_information = (ProResAspectRatio?)((currByte & 0b1111_0000) >> 4);
            frame_rate_code = (ProResFrameRate?)(currByte & 0b0000_1111);

            color_primaries = reader.ReadByte();
            transfer_characteristic = reader.ReadByte();
            matrix_coefficients = reader.ReadByte();

            alpha_channel_type = (reader.ReadByte() & 0x0000_1111) >> 4;

            reader.ReadByte(); // skip one byte (reserved byte)

            currByte = reader.ReadByte();
            load_luma_quantization_matrix = (currByte & 0b0000_0010 >> 1) == 1 ? true : false;
            load_chroma_quantization_matrix = (currByte & 0b0000_0001) == 1 ? true : false;

            if (load_luma_quantization_matrix)
            {
                luma_quantization_matrix = GetMatrix();
            }

            if (load_chroma_quantization_matrix)
            {
                chroma_quantization_matrix = GetMatrix();
            }
        }

        private byte[,] GetMatrix()
        {
            byte[,] retval = new byte[8, 8];

            for (int v = 0; v < 8; v++)
            {
                for (int u = 0; u < 8; u++)
                {
                    retval[v, u] = reader.ReadByte();
                }
            }
            return retval;
        }

        private long CalculateStuffingSize()
        {
            var frameDataSize = 4 + 4 + frame_header_size + FirstPicture.picture_size;
            if (interlace_mode == ProResInterlaceMode.Interlaced_TFF || interlace_mode == ProResInterlaceMode.Interlaced_BFF)
            {
                frameDataSize += SecondPicture.picture_size;
            }
            return frame_size - frameDataSize;
        }

        private bool ParseStuffing(long stuffing_size)
        {
            for (long m = 0; m < stuffing_size; m++)
            {
                if (reader.ReadByte() != 0x00)
                {
                    return false;
                }
            }
            return true;
        }

        public class Picture
        {
            protected readonly BinaryReader reader;
            protected readonly Frame frame;

            public TemporalOrder TemporalOrder { get; init; }

            public int picture_header_size { get; private set; }
            public ushort picture_vertical_size { get; private set; }
            public ushort width_in_mb { get; private set; }
            public ushort height_in_mb { get; private set; }
            public List<ushort> slice_size_in_mb { get; private set; } = new List<ushort>();
            public int number_of_slices_per_mb_row { get; private set; }
            public ushort[,] coded_size_of_slice { get; private set; }
            public uint picture_size { get; private set; }
            public ushort deprecated_number_of_slices { get; private set; }
            public int log2_desired_slice_size_in_mb { get; private set; }

            public Picture(BinaryReader _reader, Frame _frame, TemporalOrder to)
            {
                reader = _reader;
                frame = _frame;
                TemporalOrder = to;

                ParsePictureHeader();
                CalculatePicture_vertical_size(TemporalOrder);
                CalculateHeight_in_mb();
                CalculateWidth_in_mb();
                CalculateSliceSize();
                ParseSliceTable();
                for (ushort i = 0; i < height_in_mb; i++)
                {
                    for (int j = 0; j < number_of_slices_per_mb_row; j++)
                    {
                        ParseSlice(i, j);
                    }
                }
            }


            public override string ToString()
            {

                return $"{height_in_mb} × {width_in_mb} macroblocks";
            }

            private void ParsePictureHeader()
            {
                byte currByte = reader.ReadByte();
                picture_header_size = (currByte & 0b1111_1000) >> 3;
                picture_size = reader.ReadUInt32();
                deprecated_number_of_slices = reader.ReadUInt16();
                currByte = reader.ReadByte();
                log2_desired_slice_size_in_mb = (currByte & 0b0011_0000) >> 4;
            }

            private void CalculatePicture_vertical_size(TemporalOrder temporalOrder)
            {
                if (frame.interlace_mode == 0)
                {
                    picture_vertical_size = frame.vertical_size;
                }
                else
                {
                    ushort topFieldVerticalSize = (ushort)((frame.vertical_size + 1) / 2);
                    ushort bottomFieldVerticalSize = (ushort)(frame.vertical_size / 2);

                    picture_vertical_size =
                        frame.interlace_mode == ProResInterlaceMode.Interlaced_TFF && temporalOrder == TemporalOrder.First ||
                        frame.interlace_mode == ProResInterlaceMode.Interlaced_BFF && temporalOrder == TemporalOrder.Second
                    ?
                        topFieldVerticalSize
                    :
                        bottomFieldVerticalSize;
                }
            }

            /// <summary>
            ///  The width of the encoded picture in macroblocks. Derived from horizontal_size
            /// </summary>
            private void CalculateWidth_in_mb()
            {
                width_in_mb = (ushort)((frame.horizontal_size + 15) / 16);
            }

            /// <summary>
            /// The height of the encoded picture in macroblocks. Derived from picture_vertical_size
            /// </summary>
            private void CalculateHeight_in_mb()
            {
                height_in_mb = (ushort)((picture_vertical_size + 15) / 16);
            }

            private void ParseSlice(ushort i, int j)
            {
                ParseSliceHeader();
            }

            private void ParseSliceHeader()
            {

            }

            private void ParseSliceTable()
            {
                coded_size_of_slice = new ushort[height_in_mb, number_of_slices_per_mb_row];
                for (ushort i = 0; i < height_in_mb; i++)
                {
                    for (ushort j = 0; j < number_of_slices_per_mb_row; j++)
                    {
                        coded_size_of_slice[i, j] = reader.ReadUInt16();
                    }
                }
            }

            /// <summary>
            /// The height of the picture in luma samples. Derived from vertical_size and interlace_mode
            /// </summary>
            /// <param name="temporalOrder"></param>


            private void CalculateSliceSize()
            {
                int j = 0;
                ushort sliceSize = (ushort)(1 << log2_desired_slice_size_in_mb);
                ushort numMbsRemainingInRow = width_in_mb;
                do
                {
                    while (numMbsRemainingInRow >= sliceSize)
                    {
                        slice_size_in_mb.Add(sliceSize);
                        numMbsRemainingInRow -= sliceSize;
                    }
                    sliceSize /= 2;
                } while (numMbsRemainingInRow > 0);
                number_of_slices_per_mb_row = slice_size_in_mb.Count();
            }
        }
    }



    public enum ProResChromaFormat
    {
        Reserved_0 = 0x00,
        Reserved_1 = 0x01,
        ChromaFormat_422 = 0x02,
        ChromaFormat_444 = 0x03,
    }

    public enum ProResInterlaceMode
    {
        Progressive = 0x00,
        Interlaced_TFF = 0x01,
        Interlaced_BFF = 0x02,
        Reserved = 0x03
    }

    public enum ProResAspectRatio
    {

        Unknown_Unspecified = 0x00,
        Square_pixels = 0x01,
        AspectRatio_4_3 = 0x02,
        AspectRatio_16_9 = 0x03,
        Reserved_4 = 0x04,
        Reserved_5 = 0x05,
        Reserved_6 = 0x06,
        Reserved_7 = 0x07,
        Reserved_8 = 0x08,
        Reserved_9 = 0x09,
        Reserved_10 = 0x0A,
        Reserved_11 = 0x0B,
        Reserved_12 = 0x0C,
        Reserved_13 = 0x0D,
        Reserved_14 = 0x0E,
        Reserved_15 = 0x0F,
    }

    public enum TemporalOrder
    {

        First = 0x00,
        Second = 0x01
    }

    public enum ProResFrameRate
    {
        [Description("Unknown/unspecified")]
        Unknown_Unspecified = 0x00,
        [Description("24 ÷ 1.001 (23.976\u0005)")]
        FrameRate_24_1001 = 0x01,
        [Description("24")]
        FrameRate_24 = 0x02,
        [Description("25")]
        FrameRate_25 = 0x03,
        [Description("30 ÷ 1.001 (29.97\u0005)")]
        FrameRate_30_1001 = 0x04,
        [Description("30")]
        FrameRate_30 = 0x05,
        [Description("50")]
        FrameRate_50 = 0x06,
        [Description("60 ÷ 1.001 (59.94\u0005)")]
        FrameRate_60_1001 = 0x07,
        [Description("60")]
        FrameRate_60 = 0x08,
        [Description("100")]
        FrameRate_100 = 0x09,
        [Description("120 ÷ 1.001 (119.88\u0005)")]
        FrameRate_120_1001 = 0x0A,
        [Description("120")]
        FrameRate_120 = 0x0B,
        [Description("Reserved")]
        FrameRate_Reserved1 = 0x0C,
        [Description("Reserved")]
        FrameRate_Reserved2 = 0x0D,
        [Description("Reserved")]
        FrameRate_Reserved3 = 0x0E,
        [Description("Reserved")]
        FrameRate_Reserved4 = 0x0F,
    }
}
