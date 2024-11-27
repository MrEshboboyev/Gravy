namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a user updates their name.
/// </summary>
/// <param name="Id">Unique identifier for this event instance.</param>
/// <param name="UserId">Identifier of the user whose name was updated.</param>
/// <param name="FirstName">The updated first name of the user.</param>
/// <param name="LastName">The updated last name of the user.</param>
public sealed record UserNameUpdatedDomainEvent(
    Guid Id,
    Guid UserId,
    string FirstName,
    string LastName
) : DomainEvent(Id);
