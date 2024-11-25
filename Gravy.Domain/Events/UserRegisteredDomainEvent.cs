namespace Gravy.Domain.Events;

/// <summary> 
/// Event raised when a user is registered. 
/// </summary>
public sealed record UserRegisteredDomainEvent(
    Guid Id,
    Guid UserId)
    : DomainEvent(Id)
{

}