using Gravy.Domain.Primitives;

namespace Gravy.Domain.Entities.Restaurants;

/// <summary>
/// Represents a restaurant's opening hours for a specific day of the week.
/// </summary>
public sealed class OpeningHours : Entity
{
    #region Constructors

    private OpeningHours(
        Guid id,
        Guid restaurantId,
        DayOfWeek dayOfWeek,
        TimeOnly openTime,
        TimeOnly closeTime) : base(id)
    {
        RestaurantId = restaurantId;
        DayOfWeek = dayOfWeek;
        OpenTime = openTime;
        CloseTime = closeTime;
    }

    private OpeningHours()
    {
    }

    #endregion

    #region Properties
    
    public Guid RestaurantId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly OpenTime { get; private set; }
    public TimeOnly CloseTime { get; private set; }
    
    #endregion

    #region Factory Methods
    
    public static OpeningHours Create(
        Guid id,
        Guid restaurantId,
        DayOfWeek dayOfWeek,
        TimeOnly openTime,
        TimeOnly closeTime)
    {
        return new OpeningHours(
            id,
            restaurantId,
            dayOfWeek,
            openTime,
            closeTime);
    }
    
    #endregion

    #region Own Methods
    
    public void Update(
        TimeOnly openTime,
        TimeOnly closeTime)
    {
        OpenTime = openTime;
        CloseTime = closeTime;
    }

    public bool IsOpenAt(TimeOnly time)
    {
        return time >= OpenTime && time <= CloseTime;
    }
    
    #endregion
}