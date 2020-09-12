using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Myriadbits.MXF.ConformanceValidators
{

    public abstract class Validator<T>
    {
        public string Name { get; set; }

        public Type InspectedType { get { return typeof(T); } }

        private readonly IList<IValidator<T>> validationRules = new List<IValidator<T>>();

        public PropertyRule<T, TProp> AddRule<TProp>(Func<T, TProp> inspectedProperty)
        {
            var newRule = new PropertyRule<T, TProp>(inspectedProperty);
            validationRules.Add(newRule);
            return newRule;
        }

        public virtual ValidationResult Validate(T objectToValidate)
        {
            var retval = new List<ValidationResultEntry>();

            foreach (var rule in validationRules)
            {
                var entries = rule.Validate(objectToValidate);
                retval.Add(entries);
            }

            return new ValidationResult()
            {
                Name = string.Empty,
                Entries = retval
            };
        }

        public virtual ValidationResult Validate(T objectToValidate, string resultName)
        {
            var retval = new List<ValidationResultEntry>();

            foreach (var rule in validationRules)
            {
                var entries = rule.Validate(objectToValidate);
                retval.Add(entries);
            }

            return new ValidationResult()
            {
                Name = resultName,
                Entries = retval
            };
        }
    }
}
