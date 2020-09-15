using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Myriadbits.MXF
{
    // TODO: really needed?
    public static class MXFFileExtensions
    {
        public static MXFPartition GetHeader(this MXFFile file)
        {
            return file
                    .FlatList.OfType<MXFPartition>()
                    .SingleOrDefault(p => p.PartitionType == PartitionType.Header);

        }

        public static MXFPartition GetFooter(this MXFFile file)
        {
            return file
                    .FlatList.OfType<MXFPartition>()
                    .SingleOrDefault(p => p.PartitionType == PartitionType.Footer);
        }

        public static IEnumerable<MXFPartition> GetBodies(this MXFFile file)
        {
            return file
                    .FlatList.OfType<MXFPartition>()
                    .Where(p => p.PartitionType == PartitionType.Body);
        }

        public static IEnumerable<MXFPartition> GetBodiesContainingEssences(this MXFFile file)
        {
            return file
                    .GetBodies()
                    .Where(b => b.Children.OfType<MXFEssenceElement>().Any());
        }

        public static MXFCDCIPictureEssenceDescriptor GetPictureDescriptorInHeader(this MXFFile file)
        {
            return file
                    .GetHeader()
                    .Children
                    .OfType<MXFCDCIPictureEssenceDescriptor>()
                    .SingleOrDefault();
        }

        public static IEnumerable<MXFAES3AudioEssenceDescriptor> GetAudioEssenceDescriptorsInHeader(this MXFFile file)
        {
            return file
                .GetHeader()
                .Children
                .OfType<MXFAES3AudioEssenceDescriptor>();
        }

        public static IEnumerable<MXFMaterialPackage> GetMaterialPackages(this MXFFile file)
        {
            return file.FlatList.OfType<MXFMaterialPackage>();
        }

        public static IEnumerable<MXFSourcePackage> GetSourcePackage(this MXFFile file)
        {
            return file.FlatList.OfType<MXFSourcePackage>();
        }

        public static IEnumerable<MXFMaterialPackage> GetTracks(this MXFFile file)
        {
            return file.FlatList.OfType<MXFMaterialPackage>();
        }

        public static bool IsKAGSizeOfAllPartitionsEqual(this MXFFile file, uint size)
        {
            return file.Partitions.All(p => p.KagSize == size);
        }

        public static bool AreAllPartitionsOP1a(this MXFFile file)
        {
            MXFKey op1a = new MXFKey(0x06, 0x0E, 0x2B, 0x34, 0x04, 0x01, 0x01, 0x01, 0x0D, 0x01, 0x02, 0x01, 0x01, 0x01, 0x09, 0x00);
            return file.Partitions.Select(p => p.OP).Any(s => s == op1a);
        }

        public static bool AreAllPartitionVersionsEqual(this MXFFile file, MXFVersion version)
        {
            return file
                .Partitions
                .All(p => p.Version == version);
        }

        public static bool IsFooterClosedAndComplete(this MXFFile file)
        {
            return file.GetFooter().Closed && file.GetFooter().Complete == true;
        }

        public static bool AreBodiesClosedAndComplete(this MXFFile file)
        {
            return file.GetBodies().All(b => b.Closed && b.Complete);
        }

        public static bool IsHeaderStatusValid(this MXFFile file)
        {
            return !(file.GetHeader().Closed == false  && file.GetHeader().Complete == true);
        }

        public static bool NoEssencesInHeader(this MXFFile file)
        {
            return !file.GetHeader().Children.OfType<MXFEssenceElement>().Any();
        }

        public static bool ISRIPPresent(this MXFFile file)
        {
            return file.RIP != null;
        }

        public static long GetPartitionDuration(this MXFPartition partition) {
            return partition.Children.OfType<MXFEssenceElement>().Where(e => e.IsPicture).Count();
        }

        public static bool IsPartitionDurationBetween(this MXFPartition partition, long min, long max)
        {
            return partition.GetPartitionDuration() >= min && 
                partition.GetPartitionDuration() <= max;
        }

        public static bool IsDurationOfBodiesCorrect(this MXFFile file)
        {
            return file.GetBodiesContainingEssences().All(b => b.IsPartitionDurationBetween(1, 240));
        }

        public static bool IsPrefaceVersionEqualTo(this MXFFile file, ushort version)
        {
            return file.FlatList.OfType<MXFPreface>().FirstOrDefault().Version.Value == version;
        }
    }
}
