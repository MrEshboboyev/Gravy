using Gravy.Domain.Primitives;

namespace Gravy.Domain.ValueObjects.Restaurants;

/// <summary>
/// Represents a menu's availability schedule.
/// </summary>
public sealed class AvailabilitySchedule : ValueObject
{
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }
    public List<DayOfWeek> AvailableDays { get; }

    private AvailabilitySchedule(
        TimeOnly startTime,
        TimeOnly endTime,
        List<DayOfWeek> availableDays)
    {
        StartTime = startTime;
        EndTime = endTime;
        AvailableDays = availableDays;
    }

    public static AvailabilitySchedule Create(
        TimeOnly startTime,
        TimeOnly endTime,
        List<DayOfWeek> availableDays)
    {
        return new AvailabilitySchedule(
            startTime,
            endTime,
            availableDays);
    }

    public bool IsAvailableAt(DateTime dateTime)
    {
        var time = TimeOnly.FromDateTime(dateTime);
        return AvailableDays.Contains(dateTime.DayOfWeek) &&
               time >= StartTime &&
               time <= EndTime;
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return StartTime;
        yield return EndTime;
        foreach (var day in AvailableDays)
        {
            yield return day;
        }
    }
}