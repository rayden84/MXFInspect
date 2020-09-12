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


            Debug.WriteLine(file.IsFooterClosedAndComplete());
            Debug.WriteLine(file.IsKAGSizeOfAllPartitionsEqual(512));
            Debug.WriteLine(file.AreAllPartitionsOP1a());
            Debug.WriteLine(file.ISRIPPresent());

            ValidatePictureDescriptor();

            ValidateAudioDescriptor();

            // TODO: check file structure (partitions, the beginning of the PDF specs)

            // TODO: check all video and audio essences and wrappings 

            // check all video specs
            //RuleFor(file => file.GetMXFPictureDescriptorInHeader()).SetValidator(new MXFProfile01PictureDescriptorValidator());

            // check all audio specs
            //var aes3descriptors = file.GetMXFAES3AudioEssenceDescriptor();
            //RuleForEach(file => aes3descriptors).SetValidator(new MXFProfile01AudioDescriptorValidator());

        }

        private void ValidateAudioDescriptor()
        {
            var aes3descriptors = _file.GetMXFAES3AudioEssenceDescriptors().ToList();

            foreach (var desc in aes3descriptors)
            {
                var audioDescriptorValidator = new MXFProfile01AudioValidator();
                var name = $"Audio #{aes3descriptors.IndexOf(desc)+1}";
                var validationResult = audioDescriptorValidator.Validate(desc, name);              
                ValidationResults.Add(validationResult);
            }

            //RuleForEach(file => aes3descriptors).SetValidator(new MXFProfile01AES3AudioDescriptorValidator());
        }

        private void ValidatePictureDescriptor()
        {
            var pictureDescriptor = _file.GetMXFPictureDescriptorInHeader();

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
    }
}

