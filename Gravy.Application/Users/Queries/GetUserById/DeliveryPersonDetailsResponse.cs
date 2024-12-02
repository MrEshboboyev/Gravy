namespace Gravy.Application.Users.Queries.GetUserById;

public sealed record DeliveryPersonDetailsResponse(
    Guid Id,
    Guid UserId,
    string Vehicle,
    ICollection<Guid> AssignedDeliveries,
    DateTime CreatedOnUtc);
