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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Myriadbits.MXF
{

	public class MXFISXDDataElement : MXFPack
	{
		public enum MXFANCWrappingType
		{
			Unknown = 0x00,
			VANC = 0x01,
			VANCField1 = 0x02,
			VANCField2 = 0x03,
			VANCProgressiveFrame = 0x04,
			HANCFrame = 0x11,
			HANCField1 = 0x12,
			HANCField2 = 0x13,
			HANCProgressiveFrame = 0x14
		};

		public enum MXFANCPayloadCoding
		{
			Unknown_Coding = 0,
			Coding_8_bit_luma_samples = 4,
			Coding_8_bit_color_difference_samples = 5,
			Coding_8_bit_luma_and_color_difference_samples = 6,
			Coding_10_bit_luma_samples = 7,
			Coding_10_bit_color_difference_samples = 8,
			Coding_10_bit_luma_and_color_difference_samples = 9,
			Coding_8_bit_luma_samples_with_parity_error = 10,
			Coding_8_bit_color_difference_samples_with_parity_error = 11,
			Coding_8_bit_luma_and_color_difference_samples_with_parity_error = 12,
		};
		private const string CATEGORYNAME = "ANCPacket";

		protected static Dictionary<UInt16, string> m_DIDDescription; 

		[Category(CATEGORYNAME)] 
		public UInt16 LineNumber { get; set; }
		[Category(CATEGORYNAME)]
		public MXFANCWrappingType WrappingType { get; set; }
		[Category(CATEGORYNAME)]
		public MXFANCPayloadCoding PayloadSamplingCoding { get; set; }
		[Category(CATEGORYNAME)]
		public UInt16 PayloadSampleCount { get; set; }
		[Category(CATEGORYNAME)]
		public byte DID { get; set; }
		[Category(CATEGORYNAME)]
		public byte SDID { get; set; }
		[Category(CATEGORYNAME)]
		public byte Size { get; set; }
		[Category(CATEGORYNAME)]
        [TypeConverter(typeof(ByteArrayConverter))]
		public byte[] Payload { get; set; }
		[Category(CATEGORYNAME)]
		public string ContentDescription { get; set; }

		/// <summary>
		/// Static constructor, read all DID's from file
		/// </summary>
		static MXFISXDDataElement()
		{
			m_DIDDescription = new Dictionary<ushort, string>();
			
			string allText = MXF.Properties.Resources.ANC_Identifiers;
			string[] allLines = allText.Split('\n');
			if (allLines.Count() > 0)
			{
				for (int n = 1; n < allLines.Count(); n++ ) // Start at 1, skip the header
				{
					string line = allLines[n];
					string[] parts = line.Split(';');
					if (parts.Count() > 5)
					{
						try
						{
							int value1 = 0;
							int value2 = 0;
							if (!string.IsNullOrEmpty(parts[1]))
								value1 = Convert.ToInt32(parts[1].Trim(' ', 'h'), 16);
							if (!string.IsNullOrEmpty(parts[2]))
								value2 = Convert.ToInt32(parts[2].Trim(' ', 'h'), 16);
							UInt16 combinedDID = (UInt16)((value1 << 8) | value2);
							m_DIDDescription.Add(combinedDID, string.Format("{0} ({1})", parts[5], parts[4]));
							//Debug.WriteLine("combinedDID = {0:X4}, Name = {1} ({2})", combinedDID, parts[5], parts[4]);
						}
						catch (Exception)
						{
						}
					}
				}
			}
		}

		public MXFISXDDataElement(MXFPack pack) : base(pack)
		{
			//this.Length = rLength;
			//this.Payload = reader.ReadArray(reader.ReadByte, (int)rLength);
		}

		/// <summary>
		/// Some output
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("ISXD Data Element - Line {0}, Count {1}", this.LineNumber, this.PayloadSampleCount);
		}
	}
}
