using FluentValidation;
using Myriadbits.MXF.ConformanceValidators.validation_Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Myriadbits.MXF.ConformanceValidators
{
    public class PropertyRule<T, TProp> : IPropertyRule<T, TProp>, IValidator<T>
    {
        public Func<T, TProp> InspectedProperty { get; set; }
        public string Name { get; set; }
        public object State { get; set; }
        public Func<T, bool> WhenCondition { get; set; }
        public IPropertyValidator<T, TProp> Validator { get; set; }

        // ctor
        public PropertyRule(Func<T, TProp> inspectedProperty)
        {
            InspectedProperty = inspectedProperty;
        }

        public PropertyRule<T, TProp> WithName(string ruleName)
        {
            if (Name != null)
            {
                throw new InvalidOperationException("Rule already has a name.");
            }
            Name = ruleName;
            return this;
        }

        public PropertyRule<T, TProp> WithState(object state)
        {
            if (State != null)
            {
                throw new InvalidOperationException("Rule already has a state.");
            }
            State = state;
            return this;
        }

        public PropertyRule<T, TProp> When(Func<T, bool> condition)
        {
            if (WhenCondition != null)
            {
                throw new InvalidOperationException("Rule already has an execution condition.");
            }
            WhenCondition = condition;
            return this;
        }

        public ValidationResultEntry Validate(T objectToValidate)
        {
            return Validator.Validate(objectToValidate, this);
        }
    }
}
