using FluentValidation;
using Gravy.Domain.Shared;
using MediatR;

namespace Gravy.Application.Behaviors;


/// <summary> 
/// Implements a validation behavior in the MediatR pipeline. 
/// </summary> 
/// <typeparam name="TRequest">The type of the request.</typeparam> 
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    // Constructor to initialize the behavior with validators
    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    // Handles the validation of the request and returns validation errors if any
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // validators is not exist, await next()
        if (!_validators.Any())
        {
            return await next();
        }

        // Validate the request and collect errors
        Error[] errors = _validators
            .Select(validator => validator.Validate(request))
            .SelectMany(validationResult => validationResult.Errors)
            .Where(validationFailure => validationFailure is not null)
            .Select(failure => new Error(
                failure.PropertyName,
                failure.ErrorMessage))
            .Distinct()
            .ToArray();

        // errors is found, return result with errors
        if (errors.Length != 0)
        {
            return CreateValidationResult<TResponse>(errors);
        }

        // await next()
        return await next();
    }

    // Creates a ValidationResult or ValidationResult<T> based on errors
    private static TResult CreateValidationResult<TResult>(Error[] errors)
        where TResult : Result
    {
        if (typeof(TResult) == typeof(Result))
        {
            return (ValidationResult.WithErrors(errors) as TResult)!;
        }
        object validationResult = typeof(ValidationResult<>)
             .GetGenericTypeDefinition()
             .MakeGenericType(typeof(Result).GenericTypeArguments[0])
             .GetMethod(nameof(ValidationResult.WithErrors))!
             .Invoke(null, [errors])!;
        return (TResult)validationResult;
    }
}