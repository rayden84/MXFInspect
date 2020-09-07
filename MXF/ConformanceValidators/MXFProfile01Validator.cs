using System.Collections.Generic;
using System.Linq;

namespace Myriadbits.MXF.ConformanceValidators
{

    public class MXFProfile01Validator //: AbstractValidator<MXFFile>
    {
        public IList<MyValidationResult> Report { get; private set; } = new List<MyValidationResult>();

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

            //foreach (var desc in aes3descriptors)
            //{
            //    var audioDescriptorValidator = new MXFProfile01AudioDescriptorValidator();
            //    var validationRules = audioDescriptorValidator.OfType<PropertyRule>();
            //    var validationResult = audioDescriptorValidator.Validate(desc);

            //    Report.Add(new MyValidator($"AudioDescriptor #{aes3descriptors.IndexOf(desc)}", validationRules, validationResult));
            //}

            //RuleForEach(file => aes3descriptors).SetValidator(new MXFProfile01AES3AudioDescriptorValidator());
        }

        private void ValidatePictureDescriptor()
        {
            var pictureDescriptor = _file.GetMXFPictureDescriptorInHeader();

            //var pictureDescriptorValidator = new MXFProfile01PictureDescriptorValidator();
            //var validationResult = pictureDescriptorValidator.Validate(pictureDescriptor);

            var newValidator = new MXFProfile01PictureDescriptorValidator1();
            var result = newValidator.Validate(pictureDescriptor);

            Report.Add(result);



            // TODO: protect and encapsulate new valdition classes correctly
            
            //res.Entries =
            
            //IMyValidationRule res = new MyNewValidator<MyPictureDescriptorValidator>();
            //string template = "{PropertyValue} is not a valid value.";
            //foreach (var result in res)
            //{
            //    Console.WriteLine(result.Name);
            //    Console.WriteLine(result.Passed);
            //    Console.WriteLine(result.HasBeenExecuted);
            //    if (result.ExpectedValues != null)
            //    {
            //        Console.WriteLine(string.Join(",", result.ExpectedValues));
            //    }

            //    Console.WriteLine(result.ActualValue);

            //}
            //var formatter = new MessageFormatter();
            //formatter.AppendArgument("PropertyValue", "some value");

            //_report.Add("picture", validationResult);
        }
    }
}
