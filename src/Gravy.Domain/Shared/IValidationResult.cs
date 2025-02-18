namespace Gravy.Domain.Shared;

/// <summary> 
/// Interface defining a validation result with errors. 
/// </summary>
public interface IValidationResult
{
    // A standard error indicating a validation problem
    public static readonly Error ValidationError = new(
        "ValidationError",
        "A vaidation problem occurred!");

    // Array of errors that occurred during validation
    Error[] Errors { get; }
}