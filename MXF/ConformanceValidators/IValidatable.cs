namespace Myriadbits.MXF.ConformanceValidators
{
    public interface IValidatable<T>
    {
        ValidationResultEntry Validate(T objectToValidate);
    }
}