using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Myriadbits.MXF.ConformanceValidators
{

    public abstract class Validator<I>
    {
        public string Name { get; set; }

        public Type InspectedType { get { return typeof(I); } }

        private readonly IList<IValidatable<I>> validationRules = new List<IValidatable<I>>();

        public MyValidationResult Validate(I objectToValidate)
        {
            var retval = new List<ValidationResultEntry>();

            foreach (var rule in validationRules)
            {
                retval.Add(rule.Validate(objectToValidate));
            }

            return new MyValidationResult(this.Name, retval);
        }

        public IValidationRule<I, TProp> AddRule<TProp>(Func<I, TProp> inspectedProperty)
        {
            var newRule = new MyValidationRule<I, TProp>(inspectedProperty);
            validationRules.Add(newRule);
            return newRule;
        }

        // nested inner class which holds the rules and validation logic
        private class MyValidationRule<T, TProp> : IValidationRule<T, TProp>, IValidatable<T>
        {
            public string Name { get; private set; }
            public object State { get; private set; }
            public Func<T, TProp> InspectedProperty { get; private set; }
            public IEnumerable<TProp> ExpectedValues { get; private set; }
            public IEqualityComparer<TProp> Comparer { get; set; }
            public Func<T, bool> Predicate { get; private set; }
            public Func<T, bool> Condition { get; private set; }
            public bool IsCheckConditionSet { get; private set; }

            public MyValidationRule(Func<T, TProp> inspectedProperty)
            {
                InspectedProperty = inspectedProperty;
            }

            public IValidationRule<T, TProp> WithName(string ruleName)
            {
                if (Name != null)
                {
                    throw new InvalidOperationException("Rule already has a name.");
                }
                Name = ruleName;
                return this;
            }

            public IValidationRule<T, TProp> WithState(object state)
            {
                if (State != null)
                {
                    throw new InvalidOperationException("Rule already has a state.");
                }
                State = state;
                return this;
            }

            public IValidationRule<T, TProp> MustSatisfy(Func<T, bool> predicate)
            {
                if (predicate == null)
                {
                    throw new ArgumentException("The predicate cannot be null.");
                }
                if (IsCheckConditionSet)
                {
                    throw new InvalidOperationException("Rule already has a check condition.");
                }
                Predicate = predicate;
                IsCheckConditionSet = true;
                return this;
            }

            public IValidationRule<T, TProp> EqualTo(TProp expectedValue)
            {
                return EqualTo(expectedValue, null);
            }

            public IValidationRule<T, TProp> EqualTo(TProp expectedValue, IEqualityComparer<TProp> comparer)
            {
                if (IsCheckConditionSet)
                {
                    throw new InvalidOperationException("Rule already has a check condition.");
                }
                ExpectedValues = new List<TProp> { expectedValue };
                Comparer = comparer;
                IsCheckConditionSet = true;
                return this;
            }

            public IValidationRule<T, TProp> EqualToOneOf(IEnumerable<TProp> expectedValues)
            {
                return EqualToOneOf(expectedValues, null);
            }

            public IValidationRule<T, TProp> EqualToOneOf(IEnumerable<TProp> expectedValues, IEqualityComparer<TProp> comparer)
            {
                if (IsCheckConditionSet)
                {
                    throw new InvalidOperationException("Rule already has a check condition.");
                }
                ExpectedValues = expectedValues;
                Comparer = comparer;
                IsCheckConditionSet = true;
                return this;
            }

            public IValidationRule<T, TProp> When(Func<T, bool> condition)
            {
                if (Condition != null)
                {
                    throw new InvalidOperationException("Rule already has an execution condition.");
                }
                Condition = condition;
                return this;
            }

            private bool ValidateRule(T objectToValidate)
            {
                var actualValue = InspectedProperty.Invoke(objectToValidate);
                // is rule properly formed
                if (IsCheckConditionSet)
                {
                    if (Predicate != null)
                    {
                        return Predicate.Invoke(objectToValidate);
                    }
                    else
                    {
                        if (Comparer != null)
                        {
                            return ExpectedValues.Contains(actualValue, Comparer);
                        }
                        else
                        {
                            return ExpectedValues.Contains(actualValue);
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Rule with name {Name} does not have a check condition.");
                }
            }

            public ValidationResultEntry Validate(T objectToValidate)
            {
                var mustBeExecuted = Condition?.Invoke(objectToValidate) ?? true;
                var actualValue = InspectedProperty.Invoke(objectToValidate);
                var expectedValues = ExpectedValues != null ? ExpectedValues.Cast<object>() : null;

                ValidationResultEntry entry;

                if (mustBeExecuted)
                {
                    var outcome = ValidateRule(objectToValidate);


                    entry = new ValidationResultEntry()
                    {
                        Name = Name,
                        State = State,
                        Passed = outcome,
                        HasBeenExecuted = true,
                        ActualValue = actualValue,
                        ExpectedValues = expectedValues,
                    };
                }
                else
                {
                    entry = new ValidationResultEntry()
                    {
                        Name = Name,
                        State = State,
                        Passed = null,
                        HasBeenExecuted = false,
                        ActualValue = actualValue,
                        ExpectedValues = expectedValues,
                    };
                }
                return entry;
            }
        }
    }
}
