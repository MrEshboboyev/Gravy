﻿namespace Gravy.Application.Restaurants.Queries.GetRestaurantById;

public sealed record RestaurantResponse(
    Guid Id,
    bool IsActive,
    string Name, 
    string Description, 
    string Email, 
    string PhoneNumber,
    string Address, 
    Guid OwnerId,
    TimeSpan[] OpeningHours, 
    IReadOnlyCollection<MenuItemResponse> MenuItems);
