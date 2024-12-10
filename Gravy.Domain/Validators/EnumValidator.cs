using Gravy.Domain.Errors;
using Gravy.Domain.Shared;

namespace Gravy.Domain.Validators;

public static class EnumValidator
{
    public static Result Validate<TEnum>(TEnum value) where TEnum : struct
    {
        if (!Enum.IsDefined(typeof(TEnum), value))
        {
            return Result.Failure(
                DomainErrors.General.InvalidEnumValue(typeof(TEnum).Name));
        }

        return Result.Success();
    }
}

