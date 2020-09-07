using System;
using System.Collections.Generic;
using System.Linq;

namespace Myriadbits.MXF.ConformanceValidators
{
    public class PredicateValidator<T, TProp> : IPropertyValidator<T, TProp>
    {
        public Func<TProp, bool> Predicate { get; protected set; }
        public bool IsCheckConditionSet { get; private set; }

        public PredicateValidator(Func<TProp, bool> predicate)
        {
            Predicate = predicate;
        }

        //public PredicateValidationRule<T, TProp> MustSatisfy(Func<TProp, bool> predicate)
        //{
        //    if (predicate == null)
        //    {
        //        throw new ArgumentException("The predicate cannot be null.");
        //    }
        //    if (IsCheckConditionSet)
        //    {
        //        throw new InvalidOperationException("Rule already has a check condition.");
        //    }
        //    ExpectedValues = null;
        //    Predicate = predicate;
        //    IsCheckConditionSet = true;
        //    return this;
        //}

        //private bool ValidateRule(T objectToValidate)
        //{
        //    var actualValue = InspectedProperty.Invoke(objectToValidate);
        //    // is rule properly formed
        //    if (IsCheckConditionSet && Predicate != null)
        //    {
        //        return Predicate.Invoke(actualValue);
        //    }
        //    else
        //    {

        //        throw new InvalidOperationException($"Rule with name {Name} does not have a check condition.");
        //    }
        //}

        public ValidationResultEntry Validate(T objectToValidate, PropertyRule<T, TProp> propertyRule)
        {
            //var mustBeExecuted = WhenCondition?.Invoke(objectToValidate) ?? true;
            //var actualValue = InspectedProperty.Invoke(objectToValidate);

            ValidationResultEntry entry = new ValidationResultEntry();

            //if (mustBeExecuted)
            //{
            //    var outcome = ValidateRule(objectToValidate);


            //    entry = new ValidationResultEntry()
            //    {
            //        Name = Name,
            //        State = State,
            //        Passed = outcome,
            //        HasBeenExecuted = true,
            //        ActualValue = actualValue,
            //        //ExpectedValues = expectedValues.Cast<object>(),
            //    };
            //}
            //else
            //{
            //    entry = new ValidationResultEntry()
            //    {
            //        Name = Name,
            //        State = State,
            //        Passed = null,
            //        HasBeenExecuted = false,
            //        ActualValue = actualValue,
            //        //ExpectedValues = expectedValues.Cast<object>(),
            //    };
            //}
            return entry;
        }
    }
}
