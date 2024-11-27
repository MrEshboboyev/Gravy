using Gravy.Domain.Enums;
using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a restaurant in the system.
/// Acts as the aggregate root for restaurant-related data.
/// </summary>
public sealed class Restaurant : AggregateRoot, IAuditableEntity
{
    private readonly List<MenuItem> _menuItems = [];

    // Constructor
    private Restaurant(Guid id, string name, string description, Email email, string phoneNumber,
        Address address, Guid ownerId, OpeningHours openingHours) : base(id)
    {
        Name = name;
        Description = description;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        OwnerId = ownerId;
        OpeningHours = openingHours;
        IsActive = true;

        RaiseDomainEvent(new RestaurantCreatedDomainEvent(
            Guid.NewGuid(),
            Id,
            Name));
    }

    private Restaurant()
    {
    }

    // Properties
    public Guid OwnerId { get; private set; } // Reference to the User entity
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Email Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public Address Address { get; private set; }
    public OpeningHours OpeningHours { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<MenuItem> MenuItems => _menuItems.AsReadOnly();
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    /// <summary>
    /// Factory method to create a new restaurant.
    /// </summary>
    public static Restaurant Create(
        Guid id,
        string name,
        string description,
        Email email,
        string phoneNumber,
        Address address,
        Guid ownerId,
        OpeningHours openingHours)
    {
        return new Restaurant(
            id,
            name,
            description,
            email,
            phoneNumber,
            address,
            ownerId,
            openingHours);
    }

    /// <summary>
    /// Updates the restaurant's details.
    /// </summary>
    public void UpdateDetails(string name, 
        string description, 
        Email email, 
        string phoneNumber, 
        Address address)
    {
        Name = name;
        Description = description;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new RestaurantUpdatedDomainEvent(
            Guid.NewGuid(), 
            Id, 
            Name));
    }

    /// <summary>
    /// Marks the restaurant as inactive.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new RestaurantDeactivatedDomainEvent(
            Guid.NewGuid(), 
            Id, 
            Name));
    }

    /// <summary>
    /// Adds a new menu item to the restaurant.
    /// </summary>
    public void AddMenuItem(
        Guid menuItemId, 
        string name, 
        string description, 
        decimal price, 
        Category category)
    {
        var menuItem = MenuItem.Create(menuItemId, Id, name, description, price, category, true);
        _menuItems.Add(menuItem);
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new MenuItemAddedDomainEvent(
            Guid.NewGuid(), 
            Id, 
            menuItemId, 
            name, 
            price, 
            category));
    }

    /// <summary>
    /// Removes a menu item from the restaurant's menu.
    /// </summary>
    public void RemoveMenuItem(Guid menuItemId)
    {
        var menuItem = _menuItems.SingleOrDefault(m => m.Id == menuItemId) 
            ?? throw new InvalidOperationException("Menu item not found.");
        _menuItems.Remove(menuItem);
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new MenuItemRemovedDomainEvent(
            Guid.NewGuid(), 
            Id, 
            menuItemId));
    }
}
