namespace Gravy.Domain.Shared;

/// <summary> 
/// Represents a validation result containing errors. 
/// </summary>
public sealed class ValidationResult : Result, IValidationResult
{
    // Constructor to initialize ValidationResult with errors
    private ValidationResult(Error[] errors)
        : base(false, IValidationResult.ValidationError)
        => Errors = errors;

    // Array of errors that occurred during validation
    public Error[] Errors { get; }

    // Factory method to create a ValidationResult with errors
    public static ValidationResult WithErrors(Error[] errors) => new(errors);
}