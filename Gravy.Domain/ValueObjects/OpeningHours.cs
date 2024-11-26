using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects;

/// <summary>
/// Represents the opening hours of a restaurant.
/// </summary>
public sealed class OpeningHours : ValueObject
{
    private OpeningHours(TimeSpan openTime, TimeSpan closeTime)
    {
        OpenTime = openTime;
        CloseTime = closeTime;
    }

    public TimeSpan OpenTime { get; }
    public TimeSpan CloseTime { get; }

    /// <summary>
    /// Creates an instance of OpeningHours.
    /// </summary>
    public static Result<OpeningHours> Create(TimeSpan openTime, TimeSpan closeTime)
    {
        if (openTime >= closeTime)
        {
            return Result.Failure<OpeningHours>(DomainErrors.OpeningHours.InvalidTimeRange);
        }

        return new OpeningHours(openTime, closeTime);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return OpenTime;
        yield return CloseTime;
    }
}
