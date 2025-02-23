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

using Myriadbits.MXF.Identifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Myriadbits.MXF.Extensions
{
    public static class MXFTrackExtensions
    {
        public static MXFSourcePackage GetContainingSourcePackage (this MXFTrack t)
        {
            return t.Parent as MXFSourcePackage;
        }

        public static MXFMaterialPackage GetContainingMaterialPackage(this MXFTrack t)
        {
            return t.Parent as MXFMaterialPackage;
        }

        public static IEnumerable<MXFTrack> GetTracks(this MXFPackage package)
        {
            return package.LogicalChildren.OfType<MXFTrack>();
        }

        public static MXFSequence GetFirstMXFSequence(this MXFTrack track)
        {
            return track.LogicalChildren.OfType<MXFSequence>().FirstOrDefault();
        }

        public static string GetTrackInfo(this MXFTrack track)
        {
            try
            {
                if (track != null)
                {
                    StringBuilder sb = new StringBuilder();
                    MXFSequence seq = track.GetFirstMXFSequence();
                    sb.Append($"Id: {track.TrackID}, ");
                    if (seq != null && seq.DataDefinition != null && seq.DataDefinition is UL ul)
                    {
                        sb.Append($"{ul?.Name}, ");
                    }

                    if (track is MXFTimelineTrack timeLineTrack)
                    {
                        sb.Append($"EditRate: {timeLineTrack.EditRate.ToString(true)}");
                    }
                    if (!string.IsNullOrEmpty(track.TrackName))
                    {
                        sb.Append($" {{{track.TrackName}}}");
                    }
                    return sb.ToString();

                }
                return "";
            }
            catch (Exception)
            {
                // TODO log this error
                return "Unable to retrieve track info";
            }
        }
    }
}
