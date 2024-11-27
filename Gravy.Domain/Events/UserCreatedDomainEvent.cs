using Gravy.Domain.Primitives;

namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a new user is created in the system.
/// </summary>
/// <param name="Id">Unique identifier for this event instance.</param>
/// <param name="UserId">Identifier of the newly created user.</param>
/// <param name="Email">Email address of the user.</param>
public sealed record UserCreatedDomainEvent(Guid Id, Guid UserId, string Email) : DomainEvent(Id);
