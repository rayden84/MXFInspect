using System;
using System.Collections.Generic;
using System.Linq;

namespace Myriadbits.MXF.ConformanceValidators
{
    public class EqualToValidator<T, TProp> : IPropertyValidator<T, TProp>
    {
        public IList<TProp> ExpectedValues { get; private set; }
        public IEqualityComparer<TProp> Comparer { get; set; }

        public EqualToValidator(IEnumerable<TProp> expectedValues, IEqualityComparer<TProp> comparer = null)
        {
            ExpectedValues = expectedValues.ToList();
            Comparer = comparer;
        }

        public ValidationResultEntry Validate(T objectToValidate, PropertyRule<T, TProp> rule)
        {
            object actualValue = rule.InspectedProperty.Invoke(objectToValidate);
            bool mustbeExecuted = rule.WhenCondition?.Invoke(objectToValidate) ?? true;
            bool? passed;
            if (mustbeExecuted)
            {
                passed = ExpectedValues?.Contains((TProp)actualValue, Comparer) ?? false;
            }
            else
            {
                passed = null;
            }

            return new ValidationResultEntry
            {
                Name = rule.Name,
                ActualValue = actualValue,
                ExpectedValues = this.ExpectedValues.Cast<object>(),
                Passed = passed,
                HasBeenExecuted = mustbeExecuted
            };

        }
    }
}
