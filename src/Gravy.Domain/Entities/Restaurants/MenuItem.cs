using Gravy.Domain.Enums.Restaurants;
using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects.Restaurants;

namespace Gravy.Domain.Entities.Restaurants;

/// <summary>
/// Represents a menu item in a restaurant's menu.
/// </summary>
public sealed class MenuItem : Entity
{
    #region Constructors
    private MenuItem(
        Guid id,
        Guid menuId,
        string name,
        string description,
        decimal price,
        string category,
        List<string> ingredients,
        NutritionalInfo nutritionalInfo,
        string image,
        MenuItemStatus status,
        List<string> customizations,
        SpicyLevel spicyLevel) : base(id)
    {
        MenuId = menuId;
        Name = name;
        Description = description;
        Price = price;
        Category = category;
        Ingredients = ingredients;
        NutritionalInfo = nutritionalInfo;
        Image = image;
        Status = status;
        Customizations = customizations;
        SpicyLevel = spicyLevel;
    }

    private MenuItem()
    {
    }
    #endregion

    #region Properties
    public Guid MenuId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public string Category { get; private set; }
    public List<string> Ingredients { get; private set; }
    public NutritionalInfo NutritionalInfo { get; private set; }
    public string Image { get; private set; }
    public MenuItemStatus Status { get; private set; }
    public List<string> Customizations { get; private set; }
    public SpicyLevel SpicyLevel { get; private set; }
    #endregion

    #region Factory Methods
    public static MenuItem Create(
        Guid id,
        Guid menuId,
        string name,
        string description,
        decimal price,
        string category,
        List<string> ingredients,
        NutritionalInfo nutritionalInfo,
        string image,
        List<string> customizations,
        SpicyLevel spicyLevel)
    {
        return new MenuItem(
            id,
            menuId,
            name,
            description,
            price,
            category,
            ingredients,
            nutritionalInfo,
            image,
            MenuItemStatus.Available,
            customizations,
            spicyLevel);
    }
    #endregion

    #region Own Methods
    public void Update(
        string name,
        string description,
        decimal price,
        string category,
        List<string> ingredients,
        NutritionalInfo nutritionalInfo,
        string image,
        List<string> customizations,
        SpicyLevel spicyLevel)
    {
        Name = name;
        Description = description;
        Price = price;
        Category = category;
        Ingredients = ingredients;
        NutritionalInfo = nutritionalInfo;
        Image = image;
        Customizations = customizations;
        SpicyLevel = spicyLevel;
    }

    public void UpdateStatus(MenuItemStatus status)
    {
        Status = status;
    }
    #endregion
}