using Gravy.Application.Restaurants.Commands.ActivateRestaurant;
using Gravy.Application.Restaurants.Commands.CreateRestaurant;
using Gravy.Application.Restaurants.Commands.DeactivateRestaurant;
using Gravy.Application.Restaurants.Commands.MenuItems.AddMenuItem;
using Gravy.Application.Restaurants.Commands.MenuItems.RemoveMenuItem;
using Gravy.Application.Restaurants.Commands.MenuItems.UpdateMenuItem;
using Gravy.Application.Restaurants.Commands.UpdateRestaurant;
using Gravy.Application.Restaurants.Queries.GetRestaurantById;
using Gravy.Application.Restaurants.Queries.GetRestaurantOwner;
using Gravy.Application.Restaurants.Queries.MenuItems.GetMenuItemsByCategory;
using Gravy.Application.Restaurants.Queries.SearchRestaurantsByName;
using Gravy.Domain.Enums;
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
    #region User Claims

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    #endregion

    #region Restaurant

    #region Get

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRestaurantById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetRestaurantByIdQuery(id);
        
        var response = await Sender.Send(query, cancellationToken);
        
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> SearchRestaurantsByName(
        [FromQuery] string name,
        CancellationToken cancellationToken)
    {
        var query = new SearchRestaurantsByNameQuery(name);
        
        var response = await Sender.Send(query, cancellationToken);
        
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }
    
    #endregion

    #region Create/Update

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

        var result = await Sender.Send(command, cancellationToken);
        
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return CreatedAtAction(
           nameof(GetRestaurantById),
           new { id = result.Value },
           result.Value);
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

        var result = await Sender.Send(command, cancellationToken);
        
        return result.IsFailure ? HandleFailure(result) : NoContent();
    }
    
    #endregion

    #region Activate/Deactivate

    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> ActivateRestaurant(Guid id, 
        CancellationToken cancellationToken)
    {
        var command = new ActivateRestaurantCommand(id);

        var result = await Sender.Send(command, cancellationToken);
        
        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateRestaurant(Guid id, 
        CancellationToken cancellationToken)
    {
        var command = new DeactivateRestaurantCommand(id);

        var result = await Sender.Send(command, cancellationToken);
        
        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    #endregion

    #endregion

    #region MenuItem related

    #region Get

    [AllowAnonymous]
    [HttpGet("{restaurantId:guid}/menu-items")]
    public async Task<IActionResult> GetMenuItemsByCategory(
        Guid restaurantId,
        [FromQuery] Category category, 
        CancellationToken cancellationToken)
    {
        var query = new GetMenuItemsByCategoryQuery(restaurantId, category);

        var response = await Sender.Send(query, cancellationToken);
        
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    [AllowAnonymous]
    [HttpGet("{restaurantId:guid}/owner")]
    public async Task<IActionResult> GetRestaurantOwner(
        Guid restaurantId,
        CancellationToken cancellationToken)
    {
        var query = new GetRestaurantOwnerQuery(restaurantId);
        
        var response = await Sender.Send(query, cancellationToken);
        
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    #endregion

    #region Add/Update/Remove

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

        var result = await Sender.Send(command, cancellationToken);
        
        return result.IsFailure ? HandleFailure(result) : NoContent();
    }


    [HttpPut("{restaurantId:guid}/menu-items/{menuItemId:guid}")]
    public async Task<IActionResult> UpdateMenuItem(
        Guid restaurantId,
        Guid menuItemId,
        [FromBody] UpdateMenuItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMenuItemCommand(
            restaurantId,
            menuItemId,
            request.Name,
            request.Description,
            request.Price,
            request.Category,
            request.IsAvailable);

        var result = await Sender.Send(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    [HttpDelete("{restaurantId:guid}/menu-items/{menuItemId:guid}")]
    public async Task<IActionResult> RemoveMenuItem(
        Guid restaurantId,
        Guid menuItemId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveMenuItemCommand(restaurantId, menuItemId);
        
        var result = await Sender.Send(command, cancellationToken);
        
        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    #endregion

    #endregion
}
