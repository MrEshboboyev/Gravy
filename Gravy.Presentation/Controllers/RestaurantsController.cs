﻿using Gravy.Application.Restaurants.Commands.AddMenuItem;
using Gravy.Application.Restaurants.Commands.CreateRestaurant;
using Gravy.Application.Restaurants.Commands.DeactivateRestaurant;
using Gravy.Application.Restaurants.Commands.RemoveMenuItem;
using Gravy.Application.Restaurants.Commands.UpdateRestaurant;
using Gravy.Application.Restaurants.Queries.GetRestaurantById;
using Gravy.Application.Restaurants.Queries.SearchRestaurantsByName;
using Gravy.Domain.Shared;
using Gravy.Presentation.Abstractions;
using Gravy.Presentation.Contracts.Restaurants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gravy.Presentation.Controllers;

[Authorize]
[Route("api/restaurants")]
public sealed class RestaurantsController(ISender sender) : ApiController(sender)
{
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRestaurantById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetRestaurantByIdQuery(id);
        Result<RestaurantResponse> response = await Sender.Send(query, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> SearchRestaurantsByName(
        [FromQuery] string name,
        CancellationToken cancellationToken)
    {
        var query = new SearchRestaurantsByNameQuery(name);
        Result<RestaurantListResponse> response = await Sender.Send(query, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRestaurant(
        [FromBody] CreateRestaurantRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateRestaurantCommand(
            request.Name,
            request.Description,
            request.Email,
            request.PhoneNumber,
            request.Address,
            GetUserId(),
            request.OpeningHours);

        Result<Guid> result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return CreatedAtAction(
           nameof(GetRestaurantById),
           new { id = result.Value },
           result.Value);
    }

    [HttpPost("{restaurantId:guid}/menu-items")]
    public async Task<IActionResult> AddMenuItem(
        Guid restaurantId,
        [FromBody] AddMenuItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddMenuItemCommand(
            restaurantId,
            request.Name,
            request.Description,
            request.Price, 
            request.Category);

        Result result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }

    [HttpDelete("{restaurantId:guid}/menu-items/{menuItemId:guid}")]
    public async Task<IActionResult> RemoveMenuItem(
        Guid restaurantId,
        Guid menuItemId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveMenuItemCommand(restaurantId, menuItemId);
        Result result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateRestaurant(
        Guid id,
        [FromBody] UpdateRestaurantRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRestaurantCommand(
            id,
            request.Name,
            request.Description,
            request.Email,
            request.PhoneNumber,
            request.Address);

        Result result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeactivateRestaurant(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeactivateRestaurantCommand(id);
        Result result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }
}