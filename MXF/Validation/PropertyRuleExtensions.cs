using System;
using System.Collections.Generic;
using System.Text;

namespace Myriadbits.MXF.ConformanceValidators.validation_Framework
{
    public static class PropertyRuleExtensions
    {
        public static void MustSatisfy<T, TProp>(this PropertyRule<T, TProp> rule, Func<TProp, bool> predicate)
        {
            var validator = new PredicateValidator<T, TProp>(predicate);
            rule.Validator = validator;
        }

        public static void EqualTo<T, TProp>(this PropertyRule<T, TProp> rule, TProp expectedValue)
        {
            rule.EqualTo(expectedValue, null);
        }

        public static void EqualTo<T, TProp>(this PropertyRule<T, TProp> rule, TProp expectedValue, IEqualityComparer<TProp> comparer)
        {
            var validator = new EqualToValidator<T, TProp>(new List<TProp> { expectedValue }, comparer);
            rule.Validator = validator;
        }

        public static void EqualToOneOf<T, TProp>(this PropertyRule<T, TProp> rule, IEnumerable<TProp> expectedValues)
        {
            rule.EqualToOneOf(expectedValues, null);
        }

        public static void EqualToOneOf<T, TProp>(this PropertyRule<T, TProp> rule, IEnumerable<TProp> expectedValues, IEqualityComparer<TProp> comparer)
        {
            var validator = new EqualToValidator<T, TProp>(expectedValues, comparer);
            rule.Validator = validator;
        }
    }
}
