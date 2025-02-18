namespace Gravy.Application.Users.Queries.GetUserById;

public sealed record CustomerDetailsResponse(
    Guid Id,
    Guid UserId,
    string DefaultDeliveryAddress,
    ICollection<Guid> FavoriteRestaurants,
    DateTime CreatedOnUtc);
