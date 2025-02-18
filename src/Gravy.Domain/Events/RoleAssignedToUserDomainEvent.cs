namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a role is assigned to a user.
/// </summary>
/// <param name="Id">Unique identifier for this event instance.</param>
/// <param name="UserId">Identifier of the user to whom the role was assigned.</param>
/// <param name="RoleId">Identifier of the assigned role.</param>
public sealed record RoleAssignedToUserDomainEvent(Guid Id, Guid UserId, int RoleId) : DomainEvent(Id);
