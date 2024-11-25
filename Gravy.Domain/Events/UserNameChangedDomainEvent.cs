namespace Gravy.Domain.Events;

/// <summary> 
/// Event raised when a user's name is changed. 
/// </summary>
public sealed record UserNameChangedDomainEvent(Guid Id, Guid MemberId) : DomainEvent(Id);