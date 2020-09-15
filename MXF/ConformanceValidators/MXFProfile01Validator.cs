using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Myriadbits.MXF.ConformanceValidators
{

    public class MXFProfile01Validator
    {
        public IList<ValidationResult> ValidationResults { get; private set; } = new List<ValidationResult>();

        private MXFFile _file = null;

        public MXFProfile01Validator(MXFFile file)
        {
            _file = file;


            ValidateStructure();

            // ValidateIndexTable(Segment)
            
            ValidatePicture();

            ValidateAudio();

            // Validate timelinetrack

            // validate ancillary data

            // validate Other (dark metadata)

        }
        private void ValidateStructure()
        {
            var validator = new MXFProfile01StructureValidator(_file);
            var result = validator.Validate(_file, "General");

            ValidationResults.Add(result);
        }


        private void ValidatePicture()
        {
            var pictureDescriptor = _file.GetPictureDescriptorInHeader();

            //var pictureDescriptorValidator = new MXFProfile01PictureDescriptorValidator();
            //var validationResult = pictureDescriptorValidator.Validate(pictureDescriptor);

            var validator = new MXFProfile01PictureDescriptorValidator1(pictureDescriptor);
            var result = validator.Validate(pictureDescriptor, "Video");
            var localtags = pictureDescriptor.Children?.OfType<MXFLocalTag>() ?? null;

            var tagsvalidator = new MXFProfile01PictureValidator(pictureDescriptor);
            var tagsResult = tagsvalidator.Validate(localtags);

            result.Join(tagsResult.Entries);

            ValidationResults.Add(result);
        }


        private void ValidateAudio()
        {
            var aes3descriptors = _file.GetAudioEssenceDescriptorsInHeader().ToList();

            foreach (var desc in aes3descriptors)
            {
                var audioDescriptorValidator = new MXFProfile01AudioValidator();
                var name = $"Audio #{aes3descriptors.IndexOf(desc)+1}";
                var validationResult = audioDescriptorValidator.Validate(desc, name);              
                ValidationResults.Add(validationResult);
            }

            //RuleForEach(file => aes3descriptors).SetValidator(new MXFProfile01AES3AudioDescriptorValidator());
        }


    }
}

