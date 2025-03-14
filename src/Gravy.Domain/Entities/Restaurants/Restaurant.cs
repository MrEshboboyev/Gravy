using Gravy.Domain.Enums.Restaurants;
using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects.Restaurants;

namespace Gravy.Domain.Entities.Restaurants;

/// <summary>
/// Represents a restaurant in the system.
/// Acts as the aggregate root for restaurant-related data.
/// </summary>
public sealed class Restaurant : AggregateRoot, IAuditableEntity
{
    #region Private fields

    private readonly List<OpeningHours> _openingHours = [];
    private Menu _menu;
    
    #endregion

    #region Constructor
    private Restaurant(
        Guid id,
        string name,
        string description,
        RestaurantAddress address,
        RestaurantPhoneNumber phoneNumber,
        RestaurantEmail email,
        decimal averageRating,
        RestaurantStatus status,
        int deliveryRadius,
        decimal minimumOrderAmount,
        TimeSpan estimatedDeliveryTime) : base(id)
    {
        Name = name;
        Description = description;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        AverageRating = averageRating;
        Status = status;
        DeliveryRadius = deliveryRadius;
        MinimumOrderAmount = minimumOrderAmount;
        EstimatedDeliveryTime = estimatedDeliveryTime;

        RaiseDomainEvent(new RestaurantCreatedDomainEvent(
            Guid.NewGuid(),
            Id,
            Name));
    }

    private Restaurant()
    {
    }
    #endregion

    #region Properties
    public string Name { get; private set; }
    public string Description { get; private set; }
    public RestaurantAddress Address { get; private set; }
    public RestaurantPhoneNumber PhoneNumber { get; private set; }
    public RestaurantEmail Email { get; private set; }
    public decimal AverageRating { get; private set; }
    public RestaurantStatus Status { get; private set; }
    public int DeliveryRadius { get; private set; }
    public decimal MinimumOrderAmount { get; private set; }
    public TimeSpan EstimatedDeliveryTime { get; private set; }
    public IReadOnlyCollection<OpeningHours> OpeningHours => _openingHours.AsReadOnly();
    public Menu Menu => _menu;
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    #endregion

    #region Factory methods

    /// <summary>
    /// Factory method to create a new restaurant.
    /// </summary>
    public static Restaurant Create(
        Guid id,
        string name,
        string description,
        RestaurantAddress address,
        RestaurantPhoneNumber phoneNumber,
        RestaurantEmail email,
        int deliveryRadius,
        decimal minimumOrderAmount,
        TimeSpan estimatedDeliveryTime)
    {
        return new Restaurant(
            id,
            name,
            description,
            address,
            phoneNumber,
            email,
            0, // Initial average rating
            RestaurantStatus.Active,
            deliveryRadius,
            minimumOrderAmount,
            estimatedDeliveryTime);
    }

    #endregion

    #region Own Methods

    /// <summary>
    /// Updates the restaurant's details.
    /// </summary>
    public void UpdateDetails(
        string name,
        string description,
        RestaurantAddress address,
        RestaurantPhoneNumber phoneNumber,
        RestaurantEmail email,
        int deliveryRadius,
        decimal minimumOrderAmount,
        TimeSpan estimatedDeliveryTime)
    {
        Name = name;
        Description = description;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        DeliveryRadius = deliveryRadius;
        MinimumOrderAmount = minimumOrderAmount;
        EstimatedDeliveryTime = estimatedDeliveryTime;
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new RestaurantUpdatedDomainEvent(
            Guid.NewGuid(),
            Id,
            Name));
    }

    public void UpdateStatus(RestaurantStatus status)
    {
        Status = status;
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new RestaurantStatusUpdatedDomainEvent(
            Guid.NewGuid(),
            Id,
            Status));
    }

    public void UpdateAverageRating(decimal averageRating)
    {
        AverageRating = averageRating;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void AddOpeningHours(OpeningHours openingHours)
    {
        _openingHours.Add(openingHours);
    }

    public void RemoveOpeningHours(Guid openingHoursId)
    {
        var openingHours = _openingHours.Find(oh => oh.Id == openingHoursId);
        if (openingHours != null)
        {
            _openingHours.Remove(openingHours);
        }
    }

    public void SetMenu(Menu menu)
    {
        _menu = menu;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    #endregion
}