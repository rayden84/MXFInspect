using Myriadbits.MXF.ConformanceValidators.validation_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Myriadbits.MXF.ConformanceValidators
{
    public class MXFProfile01StructureValidator : Validator<MXFFile>
    {
        public MXFProfile01StructureValidator(MXFFile file)
        {
            //File format [252W]
            AddRule(f => f)
                .WithName("File format MXF Version [252W]")
                .MustSatisfy(f => f.AreAllPartitionVersionsEqual(new MXFVersion(1, 3)));

            //File format [252W]
            AddRule(f => f)
                .WithName("File format Preface Version [252W]")
                .MustSatisfy(f => f.IsPrefaceVersionEqualTo(259));

            // Operational Pattern [25W] = OP1a(SMPTE st378: 2004)
            AddRule(f => f)
                .WithName("Operational Pattern [25W]")
                .MustSatisfy(f => f.AreAllPartitionsOP1a());

            // Is RIP Present[200W] (Amendment 2:2012 to SMPTE st377 - 1:2011)
            AddRule(f => f)
                .WithName("Is RIP Present [200W]")
                .MustSatisfy(f => f.ISRIPPresent());

            // Header Partition Status [63W]
            AddRule(f => f)
                .WithName("Header Partition Status [63W]")
                .MustSatisfy(f => f.IsHeaderStatusValid());

            // Body Partition Status [63W]
            AddRule(f => f)
                .WithName("Body Partition Status [63W]")
                .MustSatisfy(f => f.AreBodiesClosedAndComplete());

            // Body partition duration [65W, 150W] (when containing essence)
            AddRule(f => f)
                .WithName("Body partition duration [65W, 150W] (when containing essence)")
                .MustSatisfy(f => f.IsDurationOfBodiesCorrect());

            // Footer Partition Status [63W]
            AddRule(f => f)
                .WithName("Footer Partition Status [63W]")
                .MustSatisfy(f => f.IsFooterClosedAndComplete());

            // KAG Size[151W]
            AddRule(f => f)
                .WithName("KAG Size [151W]")
                .MustSatisfy(f => f.IsKAGSizeOfAllPartitionsEqual(512));

            // Header Metadata in Header Partition [117W]
            AddRule(f => f)
                .WithName("Header Metadata in Header Partition [117W]")
                .MustSatisfy(f => f.NoEssencesInHeader());

            // System Item[152W]

            // Essence location[116W]

            // Index table location[64W, 153W]

            // Header Metadata Location [155W]   

            // Descriptive Metadata[97W]

            // Random Index Pack[118W]

            // Essence Elements in Essence Container

            // Tracks in Material Package[29W]

            // Tracks in Source Package[30W]

            // Timecode Material Package[26W]

            // Timecode Source Package[26W]

            // Timecode System Item[26W]

        }

        public IEnumerable<MXFTimelineTrack> GetMaterialPackageTracks(MXFFile file)
        {
            return file
                .GetMaterialPackages()
                .FirstOrDefault()
                .Children
                .OfType<MXFTimelineTrack>();
        }
    }
}
