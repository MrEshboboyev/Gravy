using Gravy.Domain.Enums;

namespace Gravy.Domain.Events;

public sealed record MenuItemUpdatedDomainEvent(
    Guid Id,
    Guid RestaurantId,
    Guid MenuItemId,
    string Name, 
    decimal Price,
    Category Category,
    bool IsAvailable) : DomainEvent(Id);