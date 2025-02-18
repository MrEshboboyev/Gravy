using Gravy.Application.Abstractions.Messaging;
using Gravy.Application.Users.Queries.GetUserById;

namespace Gravy.Application.Restaurants.Queries.GetRestaurantOwner;

public sealed record GetRestaurantOwnerQuery(
    Guid RestaurantId) : IQuery<RestaurantOwnerResponse>;
