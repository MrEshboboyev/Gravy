using Gravy.Domain.Errors;
using Gravy.Domain.Shared;

namespace Gravy.Domain.Validators;

public static class UniquenessValidator
{
    public static Result Validate<T>(IEnumerable<T> items, 
        Func<T, string> keySelector,
        string keyValue)
    {
        bool exists = items.Any(item => keySelector(item)
            .Equals(keyValue, StringComparison.OrdinalIgnoreCase)); 
        
        return exists 
            ? Result.Failure(
                DomainErrors.General.DuplicateValue(typeof(T).Name, keyValue)) 
            : Result.Success();
    }
}

