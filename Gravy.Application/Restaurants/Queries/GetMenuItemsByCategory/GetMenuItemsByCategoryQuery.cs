﻿using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Enums;

namespace Gravy.Application.Restaurants.Queries.GetMenuItemsByCategory;

public sealed record GetMenuItemsByCategoryQuery(
    Guid RestaurantId,
    Category Category) : IQuery<MenuItemListResponse>;