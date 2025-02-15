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
    public class MXFProResEssenceInfo : MXFEssenceInfoBase
    {
        public static readonly ImmutableArray<byte> ICPF = ImmutableArray.Create<byte>(0x69, 0x63, 0x70, 0x66);

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Frame Frame { get; private set; }

        public MXFProResEssenceInfo(MXFEssenceElement el) : base(el)
        {
            using (reader)
            {
                Frame = new Frame(reader);
            }
        }

        public override string ToString()
        {
            return $"Apple ProRes {Frame?.ToString() ?? string.Empty}";
        }

    }

    public class Frame
    {
        protected readonly BinaryReader reader;

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Picture FirstPicture { get; private set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Picture SecondPicture { get; private set; }

        public uint FrameSize { get; private set; }
        public ushort FrameHeaderSize { get; private set; }

        [TypeConverter(typeof(ByteArrayConverter))]
        public byte[] FrameIdentifier { get; private set; }
        public byte BitstreamVersion { get; private set; }

        [TypeConverter(typeof(ByteArrayConverter))]
        public byte[] EncoderIdentifier { get; private set; }
        public ushort HorizontalSize { get; private set; }
        public ushort VerticalSize { get; private set; }

        [TypeConverter(typeof(EnumDescriptionConverter))]
        public ProResAspectRatio AspectRatio { get; private set; }

        [TypeConverter(typeof(EnumDescriptionConverter))]
        public ProResChromaFormat ChromaFormat { get; private set; }

        [TypeConverter(typeof(EnumDescriptionConverter))]
        public ProResInterlaceMode InterlaceMode { get; private set; }

        [TypeConverter(typeof(EnumDescriptionConverter))]
        public ProResFrameRate FrameRate { get; private set; }

        [TypeConverter(typeof(EnumDescriptionConverter))]
        public ProResColorPrimaries ColorPrimaries { get; private set; }

        [TypeConverter(typeof(EnumDescriptionConverter))]
        public ProResTransferCharacteristic TransferCharacteristic { get; private set; }

        [TypeConverter(typeof(EnumDescriptionConverter))]
        public ProResMatrixCoefficients MatrixCoefficients { get; private set; }

        [TypeConverter(typeof(EnumDescriptionConverter))]
        public ProResAlphaChannelType? AlphaChannelType { get; private set; }
        public bool LoadLumaQuantizationMatrix { get; private set; }
        public bool LoadChromaQuantizationMatrix { get; private set; }
        public long StuffingSize { get; private set; }

        public byte[,] chroma_quantization_matrix;

        public byte[,] luma_quantization_matrix;

        public Frame(BinaryReader _reader)
        {
            reader = _reader;
            FrameSize = reader.ReadUInt32();
            FrameIdentifier = reader.ReadBytes(4); // should be 0x69637066 = ICPF

            if (FrameIdentifier.SequenceEqual(MXFProResEssenceInfo.ICPF))
            {
                ParseFrameHeader();

                // save reader pos
                long readerPos = reader.BaseStream.Position;

                FirstPicture = new Picture(reader, this, TemporalOrder.First);
                if (InterlaceMode == ProResInterlaceMode.Interlaced_TFF || InterlaceMode == ProResInterlaceMode.Interlaced_BFF)
                {
                    // position reader right after first picture
                    reader.BaseStream.Seek(readerPos + FirstPicture.PictureSize, SeekOrigin.Begin);
                    SecondPicture = new Picture(reader, this, TemporalOrder.Second);
                }
                StuffingSize = CalculateStuffingSize();

                if (StuffingSize > 0)
                {
                    ParseStuffing(StuffingSize);
                }
            }
        }

        public override string ToString()
        {
            return $"{VerticalSize} × {HorizontalSize} {InterlaceMode}";
        }

        private void ParseFrameHeader()
        {
            FrameHeaderSize = reader.ReadUInt16();
            reader.ReadByte(); // skip one byte (reserved byte)
            BitstreamVersion = reader.ReadByte();
            EncoderIdentifier = reader.ReadBytes(4);
            HorizontalSize = reader.ReadUInt16();
            VerticalSize = reader.ReadUInt16();

            byte currByte = reader.ReadByte();
            ChromaFormat = (ProResChromaFormat)((currByte & 0b1100_0000) >> 6);
            InterlaceMode = (ProResInterlaceMode)((currByte & 0b0000_1100) >> 2);

            currByte = reader.ReadByte();
            AspectRatio = (ProResAspectRatio)((currByte & 0b1111_0000) >> 4);
            FrameRate = (ProResFrameRate)(currByte & 0b0000_1111);

            ColorPrimaries = GetColorPrimaries(reader.ReadByte());
            TransferCharacteristic = GetTransferCharacteristic(reader.ReadByte());
            MatrixCoefficients = GetProResMatrixCoefficients(reader.ReadByte());

            AlphaChannelType = GetAlphaChannelType((byte)((reader.ReadByte() & 0b0000_1111) >> 4));

            reader.ReadByte(); // skip one byte (reserved byte)

            currByte = reader.ReadByte();
            LoadLumaQuantizationMatrix = (currByte & 0b0000_0010 >> 1) == 1 ? true : false;
            LoadChromaQuantizationMatrix = (currByte & 0b0000_0001) == 1 ? true : false;

            if (LoadLumaQuantizationMatrix)
            {
                luma_quantization_matrix = GetMatrix();
            }

            if (LoadChromaQuantizationMatrix)
            {
                chroma_quantization_matrix = GetMatrix();
            }
        }

        private ProResMatrixCoefficients GetProResMatrixCoefficients(byte value)
        {
            if (value >= 10)
            {
                return ProResMatrixCoefficients.Reserved_10;
            }
            else return (ProResMatrixCoefficients)value;
        }

        private ProResColorPrimaries GetColorPrimaries(byte value)
        {
            if (value > 13)
            {
                return ProResColorPrimaries.Reserved_13;
            }
            else return (ProResColorPrimaries)value;
        }

        private ProResAlphaChannelType GetAlphaChannelType(byte value)
        {
            if (value > 13)
            {
                return ProResAlphaChannelType.Reserved;
            }
            else return (ProResAlphaChannelType)value;
        }

        private ProResTransferCharacteristic GetTransferCharacteristic(byte value)
        {
            switch (value)
            {
                case 0:
                case 1:
                case 2:
                case 16:
                    return (ProResTransferCharacteristic) value;

                default:
                    return ProResTransferCharacteristic.Unknown_Unspecified_0;
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
            var frameDataSize = 4 + 4 + FrameHeaderSize + FirstPicture.PictureSize;
            if (InterlaceMode == ProResInterlaceMode.Interlaced_TFF || InterlaceMode == ProResInterlaceMode.Interlaced_BFF)
            {
                frameDataSize += SecondPicture.PictureSize;
            }
            return FrameSize - frameDataSize;
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

            public TemporalOrder TemporalOrder { get; private set; }

            public int PictureHeaderSize { get; private set; }
            public ushort PictureVerticalSize { get; private set; }
            public ushort WidthInMacroBlocks { get; private set; }
            public ushort HeightInMacroBlocks { get; private set; }
            public List<ushort> SliceSizeInMacroBlocks { get; private set; } = new List<ushort>();
            public int NumberOfSlicesPerMacroBlockRow { get; private set; }
            public ushort[,] CodedSizeofSlice { get; private set; }
            public uint PictureSize { get; private set; }
            public ushort DeprecatedNumberOfSlices { get; private set; }
            public int Log2DesiredSliceSizeInMacroBlocks { get; private set; }

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
                for (ushort i = 0; i < HeightInMacroBlocks; i++)
                {
                    for (int j = 0; j < NumberOfSlicesPerMacroBlockRow; j++)
                    {
                        ParseSlice(i, j);
                    }
                }
            }


            public override string ToString()
            {

                return $"{HeightInMacroBlocks} × {WidthInMacroBlocks} macroblocks";
            }

            private void ParsePictureHeader()
            {
                byte currByte = reader.ReadByte();
                PictureHeaderSize = (currByte & 0b1111_1000) >> 3;
                PictureSize = reader.ReadUInt32();
                DeprecatedNumberOfSlices = reader.ReadUInt16();
                currByte = reader.ReadByte();
                Log2DesiredSliceSizeInMacroBlocks = (currByte & 0b0011_0000) >> 4;
            }

            private void CalculatePicture_vertical_size(TemporalOrder temporalOrder)
            {
                if (frame.InterlaceMode == 0)
                {
                    PictureVerticalSize = frame.VerticalSize;
                }
                else
                {
                    ushort topFieldVerticalSize = (ushort)((frame.VerticalSize + 1) / 2);
                    ushort bottomFieldVerticalSize = (ushort)(frame.VerticalSize / 2);

                    PictureVerticalSize =
                        frame.InterlaceMode == ProResInterlaceMode.Interlaced_TFF && temporalOrder == TemporalOrder.First ||
                        frame.InterlaceMode == ProResInterlaceMode.Interlaced_BFF && temporalOrder == TemporalOrder.Second
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
                WidthInMacroBlocks = (ushort)((frame.HorizontalSize + 15) / 16);
            }

            /// <summary>
            /// The height of the encoded picture in macroblocks. Derived from picture_vertical_size
            /// </summary>
            private void CalculateHeight_in_mb()
            {
                HeightInMacroBlocks = (ushort)((PictureVerticalSize + 15) / 16);
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
                CodedSizeofSlice = new ushort[HeightInMacroBlocks, NumberOfSlicesPerMacroBlockRow];
                for (ushort i = 0; i < HeightInMacroBlocks; i++)
                {
                    for (ushort j = 0; j < NumberOfSlicesPerMacroBlockRow; j++)
                    {
                        CodedSizeofSlice[i, j] = reader.ReadUInt16();
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
                ushort sliceSize = (ushort)(1 << Log2DesiredSliceSizeInMacroBlocks);
                ushort numMbsRemainingInRow = WidthInMacroBlocks;
                do
                {
                    while (numMbsRemainingInRow >= sliceSize)
                    {
                        SliceSizeInMacroBlocks.Add(sliceSize);
                        numMbsRemainingInRow -= sliceSize;
                    }
                    sliceSize /= 2;
                } while (numMbsRemainingInRow > 0);
                NumberOfSlicesPerMacroBlockRow = SliceSizeInMacroBlocks.Count();
            }
        }
    }


    public enum ProResChromaFormat
    {
        [Description("Reserved")]
        Reserved_0 = 0x00,
        [Description("Reserved")]
        Reserved_1 = 0x01,
        [Description("4:2:2")]
        ChromaFormat_422 = 0x02,
        [Description("4:4:4")]
        ChromaFormat_444 = 0x03,
    }

    public enum ProResTransferCharacteristic
    {
        [Description("Unknown/Unspecified")]
        Unknown_Unspecified_0 = 0x00,
        [Description("ITU-R BT.601/BT.709/BT.2020")]
        ITU_R = 0x01,
        [Description("Unknown/Unspecified")]
        Unknown_Unspecified_1 = 0x02,
        [Description("SMPTE ST 2084:2014")]
        SMPTE_ST_2084_2014 = 0x16,      
        [Description("Reserved")]
        Reserved = 0x17,
    }

    public enum ProResMatrixCoefficients
    {
        [Description("Unknown/Unspecified")]
        Unknown_Unspecified = 0x00,
        [Description("ITU-R BT.709")]
        BT_709 = 0x01,
        [Description("Unknown/Unspecified")]
        Unknown = 0x02,
        [Description("Reserved")]
        Reserved_3 = 0x03,
        [Description("Reserved")]
        Reserved_4 = 0x04,
        [Description("Reserved")]
        Reserved_5 = 0x05,
        [Description("ITU-R BT.601")]
        BT_601 = 0x06,
        [Description("Reserved")]
        Reserved_7 = 0x03,
        [Description("Reserved")]
        Reserved_8 = 0x03,
        [Description("ITU-R BT.2020")]
        BT_2020 = 0x03,
        [Description("Reserved")]
        Reserved_10 = 0x10,
    }


    public enum ProResAlphaChannelType
    {
        [Description("No encoded alpha data present in bitstream")]
        NoAlpha = 0x00,
        [Description("8 bits/sample integral alpha")]
        Bits_8 = 0x01,
        [Description("16 bits/sample integral alpha")]
        Bits_16 = 0x02,
        [Description("Reserved")]
        Reserved = 0x03,
    }

    public enum ProResColorPrimaries
    {
        [Description("Unknown/Unspecified")]
        Unknown_0 = 0x00,
        [Description("ITU-R BT.709")]
        BT_709 = 0x01,
        [Description("Unknown/Unspecified")]
        Unknown_2 = 0x02,
        [Description("Reserved")]
        Reserved_3 = 0x03,
        [Description("Reserved")]
        Reserved_4 = 0x04,
        [Description("ITU-R BT.601 625")]
        BT_601_625 = 0x05,
        [Description("ITU-R BT.601 525")]
        BT_601_525 = 0x06,
        [Description("Reserved")]
        Reserved_7 = 0x07,
        [Description("Reserved")]
        Reserved_8 = 0x08,
        [Description("ITU-R BT.2020")]
        BT_2020 = 0x09,
        [Description("Reserved")]
        Reserved_10 = 0x10,
        [Description("DCI P3")]
        DCI_P3 = 0x11,
        [Description("P3 D65")]
        P3_D65 = 0x12,
        [Description("Reserved")]
        Reserved_13 = 0x13,
    }

    public enum ProResInterlaceMode
    {
        [Description("Progressive")]
        Progressive = 0x00,
        [Description("Interlaced TFF")]
        Interlaced_TFF = 0x01,
        [Description("Interlaced BFF")]
        Interlaced_BFF = 0x02,
        [Description("Reserved")]
        Reserved = 0x03
    }

    public enum ProResAspectRatio
    {
        [Description("Unknown/Unspecified")]
        Unknown_Unspecified = 0x00,
        [Description("Square Pixels")]
        Square_pixels = 0x01,
        [Description("4:3")]
        AspectRatio_4_3 = 0x02,
        [Description("16:9")]
        AspectRatio_16_9 = 0x03,
        [Description("Reserved")]
        Reserved_4 = 0x04,
        [Description("Reserved")]
        Reserved_5 = 0x05,
        [Description("Reserved")]
        Reserved_6 = 0x06,
        [Description("Reserved")]
        Reserved_7 = 0x07,
        [Description("Reserved")]
        Reserved_8 = 0x08,
        [Description("Reserved")]
        Reserved_9 = 0x09,
        [Description("Reserved")]
        Reserved_10 = 0x0A,
        [Description("Reserved")]
        Reserved_11 = 0x0B,
        [Description("Reserved")]
        Reserved_12 = 0x0C,
        [Description("Reserved")]
        Reserved_13 = 0x0D,
        [Description("Reserved")]
        Reserved_14 = 0x0E,
        [Description("Reserved")]
        Reserved_15 = 0x0F,
    }

    public enum TemporalOrder
    {

        First = 0x00,
        Second = 0x01
    }

    public enum ProResFrameRate
    {
        [Description("Unknown/Unspecified")]
        Unknown_Unspecified = 0x00,
        [Description("24 ÷ 1.001 (23.976)")]
        FrameRate_24_1001 = 0x01,
        [Description("24")]
        FrameRate_24 = 0x02,
        [Description("25")]
        FrameRate_25 = 0x03,
        [Description("30 ÷ 1.001 (29.97)")]
        FrameRate_30_1001 = 0x04,
        [Description("30")]
        FrameRate_30 = 0x05,
        [Description("50")]
        FrameRate_50 = 0x06,
        [Description("60 ÷ 1.001 (59.94)")]
        FrameRate_60_1001 = 0x07,
        [Description("60")]
        FrameRate_60 = 0x08,
        [Description("100")]
        FrameRate_100 = 0x09,
        [Description("120 ÷ 1.001 (119.88)")]
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
