using Gravy.Application.Users.Queries.GetUserById;

namespace Gravy.Application.Users.Queries.DeliveryPersons.GetAllDeliveryPersons;

public sealed record DeliveryPersonListResponse(
    IReadOnlyCollection<DeliveryPersonDetailsResponse> DeliveryPersons);

