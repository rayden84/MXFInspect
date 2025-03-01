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

using System.Linq;

namespace Myriadbits.MXF.Extensions
{
    public static class MXFPartitionExtensions
    {
        public static bool IsFooterPartition(this MXFPartition p)
        {
            return p.PartitionType == PartitionType.Footer;
        }

        public static bool IsHeaderPartition(this MXFPartition p)
        {
            return p.PartitionType == PartitionType.Header;
        }

        public static bool IsClosedAndComplete(this MXFPartition p)
        {
            return p.Status == PartitionStatus.ClosedComplete;
        }

        public static bool IsOpen(this MXFPartition p)
        {
            return
                p.Status == PartitionStatus.OpenComplete ||
                p.Status == PartitionStatus.OpenIncomplete;
        }

        public static bool IsClosed(this MXFPartition p)
        {
            return p.Status == PartitionStatus.ClosedComplete || p.Status == PartitionStatus.ClosedIncomplete;
        }

        public static bool ContainsEssences(this MXFPartition p)
        {
            return p.Children.OfType<MXFEssenceElement>().Any();
        }

        public static bool ContainsIndexTableSegments(this MXFPartition p)
        {
            return p.Children.OfType<MXFIndexTableSegment>().Any();
        }

        public static long CountPictureEssences(this MXFPartition p)
        {
            return p.Children.OfType<MXFEssenceElement>().Count(e => e.IsPicture);
        }

        public static bool IsPartitionDurationBetween(this MXFPartition partition, long min, long max)
        {
            return partition.CountPictureEssences() >= min &&
                partition.CountPictureEssences() <= max;
        }
    }
}
