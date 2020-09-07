using System;
using System.Collections.Generic;

namespace Myriadbits.MXF.ConformanceValidators
{
    public interface IPropertyRule<T, TProp>
    {
        PropertyRule<T, TProp> WithName(string ruleName);

        PropertyRule<T,TProp> WithState(object state);

        PropertyRule<T, TProp> When(Func<T, bool> condition);

    }
}
