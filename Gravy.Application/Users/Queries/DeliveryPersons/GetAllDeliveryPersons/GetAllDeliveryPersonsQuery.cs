using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Users.Queries.DeliveryPersons.GetAllDeliveryPersons;

public sealed record GetAllDeliveryPersonsQuery : IQuery<DeliveryPersonListResponse>;

