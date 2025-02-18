namespace Gravy.Domain.Shared;

/// <summary> 
/// Represents a validation result containing errors for a specific type. 
/// </summary> 
/// <typeparam name="TValue">The type of the value being validated.</typeparam>
public sealed class ValidationResult<TValue> : Result<TValue>, IValidationResult
{
    // Constructor to initialize ValidationResult with errors
    private ValidationResult(Error[] errors)
        : base(default!, false, IValidationResult.ValidationError)
        => Errors = errors;

    // Array of errors that occurred during validation
    public Error[] Errors { get; }

    // Factory method to create a ValidationResult with errors for a specific type
    public static ValidationResult<TValue> WithErrors(Error[] errors) => new(errors);
}