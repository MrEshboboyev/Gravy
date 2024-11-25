using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects;

public sealed class FirstName : ValueObject
{
    public const int MaxLength = 50;
    private FirstName(string value)
    {
        Value = value;
    }
    public string Value { get; }


    /// <summary> 
    /// Creates a FirstName instance after validating the input. 
    /// </summary> 
    /// <param name="firstName">The first name string to create the FirstName value object from.</param> 
    /// <returns>A Result object containing the FirstName value object or an error.</returns>
    public static Result<FirstName> Create(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Failure<FirstName>(DomainErrors.FirstName.Empty);
        }
        if (firstName.Length > MaxLength)
        {
            return Result.Failure<FirstName>(DomainErrors.FirstName.TooLong);
        }
        return new FirstName(firstName);
    }

    /// <summary> 
    /// Returns the atomic values of the FirstName object for equality checks. 
    /// </summary>
    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}