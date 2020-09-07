using System;
using System.Collections.Generic;

namespace Myriadbits.MXF.ConformanceValidators
{
    public interface IValidationRule<T, TProp>
    {
        IValidationRule<T,TProp> WithName(string ruleName);

        IValidationRule<T,TProp> WithState(object state);

        IValidationRule<T, TProp> When(Func<T, bool> condition);

        IValidationRule<T,TProp> MustSatisfy(Func<T, bool> predicate);

        IValidationRule<T,TProp> EqualTo(TProp expectedValue);

        IValidationRule<T, TProp> EqualTo(TProp expectedValue, IEqualityComparer<TProp> comparer);

        IValidationRule<T, TProp> EqualToOneOf(IEnumerable<TProp> expectedValues);

        IValidationRule<T, TProp> EqualToOneOf(IEnumerable<TProp> expectedValues, IEqualityComparer<TProp> comparer);
    }
}
