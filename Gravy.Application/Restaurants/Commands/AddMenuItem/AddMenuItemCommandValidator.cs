﻿using FluentValidation;

namespace Gravy.Application.Restaurants.Commands.AddMenuItem;

internal class AddMenuItemCommandValidator : AbstractValidator<AddMenuItemCommand>
{
    public AddMenuItemCommandValidator()
    {
        RuleFor(menuItem => menuItem.RestaurantId).NotEmpty();

        RuleFor(menuItem => menuItem.Name).NotEmpty();

        RuleFor(menuItem => menuItem.Price).NotEmpty();
     
        RuleFor(menuItem => menuItem.Category).NotEmpty();
    }
}