using System.Runtime.CompilerServices;

namespace Myriadbits.MXF.ConformanceValidators
{
    public interface IPropertyValidator<T, TProp>
    {
        ValidationResultEntry Validate(T objectToValidate, PropertyRule<T, TProp> rule);
    }
}