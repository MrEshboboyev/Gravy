using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a restaurant in the system.
/// </summary>
public sealed class Restaurant : AggregateRoot, IAuditableEntity
{
    // Constructor
    private Restaurant(Guid id, string name, string description, Email email, string phoneNumber,
        Address address, string ownerName, OpeningHours openingHours, bool isActive) : base(id)
    {
        Name = name;
        Description = description;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        OwnerName = ownerName;
        OpeningHours = openingHours;
        IsActive = isActive;
    }

    private Restaurant()
    {
    }

    // Properties
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Email Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public Address Address { get; private set; }
    public string OwnerName { get; private set; }
    public OpeningHours OpeningHours { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    /// <summary>
    /// Creates a new restaurant.
    /// </summary>
    public static Restaurant Create(
        Guid id,
        string name,
        string description,
        Email email, 
        string phoneNumber,
        Address address, 
        string ownerName, 
        OpeningHours openingHours, 
        bool isActive
        )
    {
        var restaurant = new Restaurant(
            id,
            name,
            description,
            email, 
            phoneNumber, 
            address, 
            ownerName, 
            openingHours,
            isActive);

        restaurant.RaiseDomainEvent(new RestaurantCreatedDomainEvent(
            Guid.NewGuid(),
            restaurant.Id));

        return restaurant;
    }

    /// <summary>
    /// Marks the restaurant as inactive.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        ModifiedOnUtc = DateTime.UtcNow;
    }
}

