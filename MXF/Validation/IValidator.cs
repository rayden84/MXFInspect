namespace Myriadbits.MXF.ConformanceValidators
{
    public interface IValidator<T>
    {
        ValidationResultEntry Validate(T objectToValidate);
    }
}