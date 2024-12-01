using Gravy.Domain.Enums;

namespace Gravy.Presentation.Contracts.Restaurants;

public sealed record UpdateMenuItemRequest(
    string Name,
    string Description,
    decimal Price,
    Category Category,
    bool IsAvailable);