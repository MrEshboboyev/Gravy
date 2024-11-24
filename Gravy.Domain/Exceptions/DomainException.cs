namespace Gravy.Domain.Exceptions;

/// <summary> 
/// Represents a base class for all domain-specific exceptions in the Gravy domain. 
/// Domain exceptions are used to handle errors and exceptional conditions that occur within the domain logic. 
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary> 
    /// Initializes a new instance of the <see cref="DomainException"/> class with a specified error message. 
    /// </summary> 
    /// <param name="message">The message that describes the error.</param>
    public DomainException(string message) : base(message)
    {

    }
}