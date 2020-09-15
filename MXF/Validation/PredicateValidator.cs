using System;
using System.Collections.Generic;
using System.Linq;

namespace Myriadbits.MXF.ConformanceValidators
{
    public class PredicateValidator<T, TProp> : IPropertyValidator<T, TProp>
    {
        public Func<T, bool> Predicate { get; protected set; }

        public PredicateValidator(Func<T, bool> predicate)
        {
            Predicate = predicate;
        }

        public ValidationResultEntry Validate(T objectToValidate, PropertyRule<T, TProp> rule)
        {
            //object actualValue = rule.InspectedProperty.Invoke(objectToValidate);
            bool mustbeExecuted = rule.WhenCondition?.Invoke(objectToValidate) ?? true;
            bool? passed;
            if (mustbeExecuted)
            {
                passed = Predicate.Invoke(objectToValidate);
            }
            else
            {
                passed = null;
            }

            return new ValidationResultEntry
            {
                Name = rule.Name,
                ActualValue = passed,
                ExpectedValues = new string[] { "true" },
                Passed = passed,
                HasBeenExecuted = mustbeExecuted
            };
        }
    }
}
