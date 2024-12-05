using Gravy.Application.Users.Queries.GetUserById;

namespace Gravy.Application.Users.Queries.GetAllDeliveryPersons;

public sealed record DeliveryPersonListResponse(
    IReadOnlyCollection<DeliveryPersonDetailsResponse> DeliveryPersons);

