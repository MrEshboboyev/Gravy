using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Users.Queries.GetAllDeliveryPersons;

public sealed record GetAllDeliveryPersonsQuery : IQuery<DeliveryPersonListResponse>;

