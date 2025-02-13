﻿using Myriadbits.MXF.Identifiers;
using Serilog;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Myriadbits.MXF.KLV
{
    public class KLVStreamReader : BinaryReader, IKLVStreamReader
    {
        private readonly Stream klvStream;

        #region Properties

        /// <summary>
        /// Returns the current file position
        /// </summary>
        public long Position => this.klvStream.Position;

        /// <summary>
        /// Returns true when the end-of-file is reached
        /// </summary>
        public bool EOF => this.klvStream.Position >= this.klvStream.Length;

        #endregion

        /// <summary>
        /// Constructor, creates the file reader
        /// </summary>
        /// <param name="reader"></param>
        public KLVStreamReader(Stream stream) : base(stream)
        {
            klvStream = stream ?? throw new ArgumentException("Stream cannot be null", nameof(stream));
        }

        /// <summary>
        /// Seeks to a position in the file
        /// </summary>
        /// <param name="newPosition">The position to head for</param>
        public void Seek(long newPosition)
        {
            this.klvStream.Seek(newPosition, SeekOrigin.Begin);
        }

        #region Basic types

        public override UInt16 ReadUInt16()
        {
            return BinaryPrimitives.ReadUInt16BigEndian(ReadBytes(2));
        }

        public override UInt32 ReadUInt32()
        {
            return BinaryPrimitives.ReadUInt32BigEndian(ReadBytes(4));
        }

        public override Int32 ReadInt32()
        {
            return BinaryPrimitives.ReadInt32BigEndian(ReadBytes(4));
        }

        public override Int64 ReadInt64()
        {
            return BinaryPrimitives.ReadInt64BigEndian(ReadBytes(8));
        }

        public override UInt64 ReadUInt64()
        {
            return BinaryPrimitives.ReadUInt64BigEndian(ReadBytes(8));
        }

        public string ReadUTF8String(long length)
        {
            return System.Text.Encoding.UTF8.GetString(this.ReadBytes((int)length));
        }

        public string ReadUTF16String(long length)
        {
            return System.Text.Encoding.BigEndianUnicode.GetString(this.ReadBytes((int)length));
        }

        #endregion

        #region Identifiers

        public AUID ReadAUID()
        {
            byte[] bytes = this.ReadBytes((int)KLVKey.KeyLengths.SixteenBytes);
            if (UL.HasValidULPrefix(bytes))
            {
                return new UL(bytes);
            }
            return new AUID(bytes);
        }


        public UL ReadUL()
        {
            return new UL(this.ReadBytes((int)KLVKey.KeyLengths.SixteenBytes));
        }

        public UMID ReadUMIDKey()
        {
            // Always read 32 bytes for UMID's 
            return new UMID(this.ReadBytes(2 * (int)KLVKey.KeyLengths.SixteenBytes));
        }

        public UUID ReadUUID()
        {
            // Always read 16 bytes for UUIDs
            return new UUID(this.ReadBytes((int)KLVKey.KeyLengths.SixteenBytes));
        }

        // TODO AUIDSet offset bug
        public IEnumerable<MXFObject> ReadAUIDSet(string singleItemName, long baseOffset, long tagLength)
        {
            if (IsSetSmallerThanTagLength(tagLength, out long setLength, out UInt32 itemCount))
            {
                if (itemCount < UInt32.MaxValue)
                {
                    for (int n = 0; n < itemCount; n++)
                    {
                        long pos = baseOffset + this.Position;
                        AUID auid = ReadAUID();
                        yield return new MXFAUID(singleItemName, pos, auid);
                    }
                }
            }
            else
            {
                throw new Exception($"Item count and/or size ({setLength} exceeding length of tag ({tagLength}).");
            }
        }

        public IEnumerable<MXFObject> GetReferenceSet<T>(string singleItemName, long baseOffset, long tagLength) where T : MXFObject
        {
            if (IsSetSmallerThanTagLength(tagLength, out long setLength, out UInt32 itemCount))
            {
                if (itemCount < UInt32.MaxValue)
                {
                    for (int n = 0; n < itemCount; n++)
                    {
                        long pos = baseOffset + this.Position;
                        yield return new MXFReference<T>(this, pos, singleItemName);
                    }
                }
            }
            else
            {
                throw new Exception($"Item count and/or size ({setLength} exceeding length of tag ({tagLength}).");
            }
        }

        private bool IsSetSmallerThanTagLength(long tagLength, out long setLength, out UInt32 itemCount)
        {
            itemCount = this.ReadUInt32();
            UInt32 itemLength = this.ReadUInt32();
            setLength = itemCount * itemLength;

            // check if the set fits into the tag length (minus the four bytes of itemCount and minus four bytes itemLength) 
            return (setLength <= tagLength - 2 * 4);
        }

        /// <summary>
        /// Reads a strong reference
        /// </summary>
        /// <param name="reader"></param>
        public MXFReference<T> ReadReference<T>(string referringItemName, long baseOffset) where T : MXFObject
        {
            return new MXFReference<T>(this, baseOffset + this.Position, referringItemName);
        }

        #endregion

        #region Reference types

        /// <summary>
        /// Reads a MXF long version in a partition
        /// </summary>
        public MXFProductVersion ReadProductVersion()
        {
            UInt16[] version = this.ReadArray(this.ReadUInt16, 4);
            MXFProductReleaseType build = (MXFProductReleaseType)this.ReadUInt16();
            return new MXFProductVersion
            {
                Major = version[0],
                Minor = version[1],
                Tertiary = version[2],
                Patch = version[3],
                Build = build,
            };
        }

        /// <summary>
        /// Reads a MXF version
        /// </summary>
        public MXFVersion ReadVersion()
        {
            var major = this.ReadByte();
            var minor = this.ReadByte();
            return new MXFVersion(major, minor);
        }

        /// <summary>
        /// Reads a timestamp
        /// </summary>
        public DateTime ReadTimeStamp()
        {
            UInt16 year = this.ReadUInt16();
            byte month = this.ReadByte();
            byte day = this.ReadByte();
            byte hour = this.ReadByte();
            byte minute = this.ReadByte();
            byte second = this.ReadByte();
            int millisecond = this.ReadByte() * 4;
            return new DateTime(year, month, day, hour, minute, second, millisecond);
        }

        /// <summary>
        /// Reads a rational
        /// </summary>
        public MXFRational ReadRational()
        {
            MXFRational rat = new MXFRational();
            rat.Num = this.ReadUInt32();
            rat.Den = this.ReadUInt32();
            return rat;
        }

        /// <summary>
        /// Reads a BCD timecode
        /// </summary>
        /// <param name="frameRate"></param>
        /// <returns></returns>
        public MXFTimeStamp ReadBCDTimeCode(double frameRate)
        {
            // TODO If MJD is set, time is milliseconds since X
            // A creation date / time stamp value should be entered in the system metadata pack according to
            // SMPTE 326M. Note that the format of this item is defined by the first byte that has the metadata
            // type value of “81h” (timecode)or “82h” (date - timecode) as defined in SMPTE 331M. In compliance
            // with SMPTE 331M, of the 17 bytes available, only the first 9 bytes are used and the last 8
            // bytes are zero filled.

            byte[] arr = this.ReadBytes(17);

            return new MXFTimeStamp(arr, frameRate);

            //MXFTimeStamp timeStamp = new MXFTimeStamp();
            //timeStamp.ParseBCDTimeCode(this, frameRate);

            //this.ReadByte(); // BG7 + BG8

            //// Read 8 dummy bytes (always zero)
            //this.ReadUInt64();

            //return timeStamp;
        }


        /// <summary>
        /// Reads a color primary
        /// </summary>
        public MXFColorPrimary ReadColorPrimary()
        {
            var xcolor = this.ReadUInt16();
            var ycolor = this.ReadUInt16();

            MXFColorPrimary colorPrimary = new MXFColorPrimary()
            {
                XColorCoordinate = xcolor,
                YColorCoordinate = ycolor
            };
            return colorPrimary;
        }

        /// <summary>
        /// Reads a RGBA component
        /// </summary>
        public MXFRGBAComponent ReadRGBAComponent()
        {
            var code = (MXFRGBAComponentKind)this.ReadByte();
            var componentSize = this.ReadByte();
            return new MXFRGBAComponent
            {
                Code = code,
                ComponentSize = componentSize
            };
        }

        /// <summary>
        /// Reads a RGBA layout
        /// </summary>
        public MXFRGBAComponent[] ReadRGBALayout()
        {
            return this.ReadArray(this.ReadRGBAComponent, 8);
        }

        /// <summary>
        /// Reads a channel status mode
        /// </summary>
        public MXFChannelStatusMode ReadChannelstatusMode()
        {
            return (MXFChannelStatusMode)this.ReadByte();
        }

        public MXFUserDataMode ReadUserDataMode()
        {
            return (MXFUserDataMode)this.ReadByte();
        }

        public T[] ReadArray<T>(Func<T> readFunction, long count)
        {
            T[] retval = new T[count];
            for (int i = 0; i < count; i++)
            {
                retval[i] = readFunction();
            }
            return retval;
        }

        #endregion
    }
}
