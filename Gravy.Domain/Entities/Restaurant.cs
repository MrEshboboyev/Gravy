using Gravy.Domain.Enums;
using Gravy.Domain.Errors;
using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;
using Gravy.Domain.Validators;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a restaurant in the system.
/// Acts as the aggregate root for restaurant-related data.
/// </summary>
public sealed class Restaurant : AggregateRoot
{
    #region Private fields
    private readonly List<MenuItem> _menuItems = [];
    #endregion

    #region Constructor
    private Restaurant(Guid id,
        string name,
        string description,
        Email email,
        string phoneNumber,
        Address address,
        Guid ownerId,
        OpeningHours openingHours) : base(id)
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
    #endregion

    #region Properties
    public Guid OwnerId { get; private set; } // Reference to the User entity
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Email Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public Address Address { get; private set; }
    public OpeningHours OpeningHours { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<MenuItem> MenuItems => _menuItems.AsReadOnly();
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ModifiedOnUtc { get; private set; }
    #endregion

    #region Factory methods
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
    #endregion

    #region Own Methods
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
    /// Marks the restaurant as active.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new RestaurantActivatedDomainEvent(
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
    #endregion

    #region Menu-Item Related
    /// <summary>
    /// Adds a new menu item to the restaurant.
    /// </summary>
    public Result<MenuItem> AddMenuItem(
        string name,
        string description,
        decimal price,
        Category category)
    {
        #region Validation Structs
        var categoryValidationResult = EnumValidator.Validate(category);
        if (categoryValidationResult.IsFailure)
        {
            return Result.Failure<MenuItem>(
                categoryValidationResult.Error);
        }
        #endregion

        #region Validation Uniqueness
        var uniquenessValidationResult = UniquenessValidator.Validate(
            _menuItems,
            m => m.Name, 
            name);
        if (uniquenessValidationResult.IsFailure)
        {
            return Result.Failure<MenuItem>(
                uniquenessValidationResult.Error);
        }
        #endregion

        #region Create MenuItem
        var menuItem = new MenuItem(
            Guid.NewGuid(),
            Id,
            name,
            description,
            price,
            category,
            true);
        #endregion

        #region Add new menuItem to _menuItems
        _menuItems.Add(menuItem);
        #endregion

        #region Domain Events
        RaiseDomainEvent(new MenuItemAddedDomainEvent(
            Guid.NewGuid(),
            Id,
            menuItem.Id,
            name,
            price,
            category));
        #endregion

        return menuItem;
    }

    /// <summary>
    /// Removes a menu item from the restaurant's menu.
    /// </summary>
    public Result RemoveMenuItem(Guid menuItemId)
    {
        var menuItem = _menuItems.SingleOrDefault(m => m.Id == menuItemId);
        if (menuItem is null)
        {
            return Result.Failure(
                DomainErrors.Restaurant.MenuItemNotFound(Id, menuItemId));
        }

        _menuItems.Remove(menuItem);
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new MenuItemRemovedDomainEvent(
            Guid.NewGuid(),
            Id,
            menuItemId));

        return Result.Success();
    }

    public Result<MenuItem> UpdateMenuItem(
        Guid menuItemId,
        string name,
        string description,
        decimal price,
        Category category,
        bool isAvailable)
    {
        #region Validation Structs
        var validationResult = EnumValidator.Validate(category);
        if (validationResult.IsFailure)
        {
            return Result.Failure<MenuItem>(
                validationResult.Error);
        }
        #endregion

        #region Validation Uniqueness
        var uniquenessValidationResult = UniquenessValidator.Validate(
            _menuItems,
            m => m.Name,
            name);
        if (uniquenessValidationResult.IsFailure)
        {
            return Result.Failure<MenuItem>(
                uniquenessValidationResult.Error);
        }
        #endregion

        #region Get MenuItem by Id
        var menuItem = _menuItems.SingleOrDefault(m => m.Id == menuItemId);
        if (menuItem is null)
        {
            return Result.Failure<MenuItem>(
                DomainErrors.Restaurant.MenuItemNotFound(Id, menuItemId));
        }
        #endregion

        #region Update this menu item details
        menuItem.UpdateDetails(
            name, 
            description, 
            price, 
            category, 
            isAvailable);
        #endregion

        #region Domain Events 
        RaiseDomainEvent(new MenuItemUpdatedDomainEvent(
            Guid.NewGuid(),
            Id,
            menuItem.Id,
            name,
            price,
            category,
            isAvailable));
        #endregion

        return menuItem;
    }
    #endregion
}
