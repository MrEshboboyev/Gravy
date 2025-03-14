using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects.Restaurants;

namespace Gravy.Domain.Entities.Restaurants;

/// <summary>
/// Represents a restaurant's menu.
/// </summary>
public sealed class Menu : Entity
{
    #region Private fields
    
    private readonly List<MenuItem> _menuItems = [];
    
    #endregion

    #region Constructors
    private Menu(
        Guid id,
        Guid restaurantId,
        AvailabilitySchedule availabilitySchedule) : base(id)
    {
        RestaurantId = restaurantId;
        AvailabilitySchedule = availabilitySchedule;
    }

    private Menu()
    {
    }
    #endregion

    #region Properties
    public Guid RestaurantId { get; private set; }
    public AvailabilitySchedule AvailabilitySchedule { get; private set; }
    public IReadOnlyCollection<MenuItem> MenuItems => _menuItems.AsReadOnly();
    #endregion

    #region Factory Methods
    public static Menu Create(
        Guid id,
        Guid restaurantId,
        AvailabilitySchedule availabilitySchedule)
    {
        return new Menu(
            id,
            restaurantId,
            availabilitySchedule);
    }
    #endregion

    #region Own Methods
    public void UpdateAvailabilitySchedule(AvailabilitySchedule availabilitySchedule)
    {
        AvailabilitySchedule = availabilitySchedule;
    }

    public void AddMenuItem(MenuItem menuItem)
    {
        _menuItems.Add(menuItem);
    }

    public void RemoveMenuItem(Guid menuItemId)
    {
        var menuItem = _menuItems.Find(mi => mi.Id == menuItemId);
        if (menuItem != null)
        {
            _menuItems.Remove(menuItem);
        }
    }

    public MenuItem GetMenuItem(Guid menuItemId)
    {
        return _menuItems.Find(mi => mi.Id == menuItemId);
    }
    #endregion
}