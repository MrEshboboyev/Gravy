using Gravy.Domain.Enums;

namespace Gravy.Application.Restaurants.Queries.GetRestaurantById;

public sealed record MenuItemResponse(
    Guid MenuItemId,
    string Name, 
    string Description, 
    decimal Price,
    Category Category,
    bool IsAvailable,
    DateTime CreatedOnUtc);

