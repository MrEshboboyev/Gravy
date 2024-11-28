using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Restaurants.Queries.GetRestaurantById;

public sealed record GetRestaurantByIdQuery(Guid RestaurantId) : IQuery<RestaurantResponse>;