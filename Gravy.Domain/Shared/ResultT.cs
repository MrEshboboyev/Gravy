namespace Gravy.Domain.Shared;

// inherit from [Result]
public class Result<TValue> : Result
{
    #region Private fields
    // Holds the value of the result
    private readonly TValue _value;
    #endregion

    // Constructor to initialize the Result<TValue> object
    protected internal Result(TValue value, bool isSuccess, Error error) : base(isSuccess, error)
        => _value = value;

    // Returns the value if the result is a success; otherwise, throws an InvalidOperationException
    public TValue Value => IsSuccess ? _value!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    #region Implicit operators
    // Implicitly converts a TValue to a Result<TValue> using the Create method
    public static implicit operator Result<TValue>(TValue value)
        => Create(value);
    #endregion
}