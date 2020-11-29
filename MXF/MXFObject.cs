﻿#region license
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
using System.Diagnostics;
using System.Linq;

namespace Myriadbits.MXF
{
	public enum MXFObjectType
	{
		Normal,
		Partition,
		Index,
		SystemItem,
		Essence,
		Meta,
		RIP,
		Filler,
		Special
	};

	public enum MXFLogType
	{
		Info,
		Warning,
		Error
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class MXFObject : MXFNode
    {
		private long m_lOffset = long.MaxValue;	// Offset in bytes from the beginning of the file
		private long m_lLength = -1;			// Length in bytes of this object
		protected MXFObjectType m_eType = MXFObjectType.Normal; // Default to normal type

		[CategoryAttribute("KLV"), ReadOnly(true)]
		public long Offset
		{
			get
			{				
				return m_lOffset;
			}
			set 
			{
				m_lOffset = value;
			}
		}

		[CategoryAttribute("KLV"), ReadOnly(true)]
		public long Length
		{
			get
			{
				if (m_lLength == -1)
				{
					// Not set, try to get the parent length
					if (this.Parent != null)
						return this.Parent.Length + this.Parent.Offset - this.Offset;
					return 0; // Unknown
				}
				else
					return m_lLength;
			}
			set
			{
				m_lLength = value;
			}
		}

		[Browsable(false)]
		public MXFObjectType Type
		{
			get
			{
				return m_eType;
			}
		}

        [Browsable(false)]
		public bool IsLoaded { get; set; }

		/// <summary>
		///Default constructor needed for derived classes such as MXFFile, ...
		/// </summary>
		/// <param name="reader"></param>
		// TODO can be removed, can it?
		public MXFObject()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reader"></param>
		public MXFObject(MXFReader reader)
		{
			this.Offset = reader.Position;
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reader"></param>
		public MXFObject(long offset)
		{
			this.Offset = offset;
		}

		/// <summary>
		/// Adds a child an sets reference of parent to this
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public MXFObject AddChild(MXFObject child)
		{
			child.Parent = this;
			this.Children.Add(child);
			if (child.Offset < this.m_lOffset)
				this.m_lOffset = child.Offset;
			return child;
		}

		public void LogInfo(string format, params object[] args) { this.Log(MXFLogType.Info, format, args); }
		public void LogWarning(string format, params object[] args) { this.Log(MXFLogType.Warning, format, args); }
		public void LogError(string format, params object[] args) { this.Log(MXFLogType.Error, format, args); }

		/// <summary>
		/// Generic log message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Log(MXFLogType type, string format, params object[] args)
		{			
			string s = string.Format("{0}: {1}",  type.ToString(), string.Format(format, args));
			Debug.WriteLine(s);
		}

		/// <summary>
		/// Some output
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (this.Children.Any())
				return this.Offset.ToString();
			return string.Format("{0} [{1} items]", this.Offset, this.Children.Count());
		}


		/// <summary>
		/// Is this object visible?
		/// </summary>
		/// <param name="skipFiller"></param>
		/// <returns></returns>
		public bool IsVisible(bool skipFiller)
		{
			if (skipFiller && this.Type == MXFObjectType.Filler)
				return false;
			return true;
		}

        /// <summary>
        /// Load the entire object from disk (when not yet loaded)
        /// </summary>
        public void Load()
		{
			if (!this.IsLoaded)
			{
				this.OnLoad();
				this.IsLoaded = true;
			}
		}

		/// <summary>
		/// Load the entire partition from disk override in derived classes when delay loading is supported
		/// </summary>
		public virtual void OnLoad()
		{
		}
	}
}
