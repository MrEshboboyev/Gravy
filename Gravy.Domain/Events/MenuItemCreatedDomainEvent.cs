namespace Gravy.Domain.Events;

/// <summary>
/// Domain event triggered when a menu item is created.
/// </summary>
public sealed record MenuItemCreatedDomainEvent(Guid Id, Guid MenuItemId) : DomainEvent(Id);

