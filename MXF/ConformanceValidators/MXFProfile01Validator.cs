using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Myriadbits.MXF.ConformanceValidators
{

    public class MXFProfile01Validator //: AbstractValidator<MXFFile>
    {
        public IList<ValidationResult> ValidationResults { get; private set; } = new List<ValidationResult>();

        private MXFFile _file = null;

        public MXFProfile01Validator(MXFFile file)
        {
            _file = file;

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
            var aes3descriptors = _file.GetMXFAES3AudioEssenceDescriptor().ToList();

            foreach (var desc in aes3descriptors)
            {
                var audioDescriptorValidator = new MXFProfile01AudioDescriptorValidator1();
                var validationResult = audioDescriptorValidator.Validate(desc);
                var name = $"AudioDescriptor #{aes3descriptors.IndexOf(desc)}";
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
            var result = validator.Validate(pictureDescriptor);
            var localtags = pictureDescriptor.Children?.OfType<MXFLocalTag>() ?? null;

            var tagsvalidator = new MXFProfile01PictureDescriptorLocalTagsValidator1(pictureDescriptor);
            var tagsResult = tagsvalidator.Validate(localtags);

            result.Join(tagsResult.Entries);

            ValidationResults.Add(result);
        }
    }
}

