
using Gravy.Domain.Enums;

namespace Gravy.Presentation.Contracts.Restaurants;

public sealed record AddMenuItemRequest(
    string Name, 
    string Description,
    decimal Price,
    Category Category);
