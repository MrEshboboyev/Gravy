using Gravy.Application.Users.Commands.CreateUser;
using Gravy.Application.Users.Commands.Customers.AddCustomerDetails;
using Gravy.Application.Users.Commands.DeliveryPersons.AddDeliveryPersonDetails;
using Gravy.Application.Users.Commands.Login;
using Gravy.Application.Users.Queries.GetUserById;
using Gravy.Presentation.Abstractions;
using Gravy.Presentation.Contracts.Users;
using Gravy.Presentation.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gravy.Presentation.Controllers;

[Route("api/users")]
public sealed class UsersController(ISender sender) : ApiController(sender)
{
    #region User Claims

    private Guid GetUserId() =>
        Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!
                .Value);

    #endregion

    #region Get

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCurrentUserById(CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(GetUserId());

        var response = await Sender.Send(query, cancellationToken);
        
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    //[HasPermission(Permission.ReadUser)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        
        var response = await Sender.Send(query, cancellationToken);
        
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    #endregion

    #region Auth related

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);

        var result = await Sender.Send(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result) : Ok(result.Value);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName);

        var result = await Sender.Send(command, cancellationToken);
        
        return result.IsFailure ? HandleFailure(result) : Ok(result.Value);
    }

    #endregion

    #region Customer related

    [Authorize]
    [HttpPost("add-customer-details")]
    public async Task<IActionResult> AddCustomerDetails(
        [FromBody] AddCustomerDetailsRequest request,
        CancellationToken cancellationToken)
    {
        // Generate random location in Tashkent for testing if not provided
        var latitude = LocationHelpers.GetRandomLatitude();
        var longitude = LocationHelpers.GetRandomLongitude();

        var command = new AddCustomerDetailsCommand(
            GetUserId(),
            request.Street,
            request.City,
            request.State,
            latitude,
            longitude);

        var result = await Sender.Send(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    #endregion

    #region Delivery Person related

    [Authorize]
    [HttpPost("add-delivery-person-details")]
    public async Task<IActionResult> AddDeliveryPersonDetails(
    [FromBody] AddDeliveryPersonDetailsRequest request,
    CancellationToken cancellationToken)
    {
        // Generate random location in Tashkent for testing if not provided
        var latitude = LocationHelpers.GetRandomLatitude();
        var longitude = LocationHelpers.GetRandomLongitude();

        var command = new AddDeliveryPersonDetailsCommand(
            GetUserId(),
            request.Type,
            request.LicensePlate,
            latitude,
            longitude);

        var result = await Sender.Send(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    #endregion
}