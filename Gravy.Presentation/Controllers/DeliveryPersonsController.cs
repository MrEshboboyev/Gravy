using Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.AddAvailability;
using Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.DeleteAvailability;
using Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.UpdateAvailability;
using Gravy.Application.Users.Queries.DeliveryPersons.GetAllDeliveryPersons;
using Gravy.Application.Users.Queries.DeliveryPersons.GetDeliveryPersonAvailabilities;
using Gravy.Presentation.Abstractions;
using Gravy.Presentation.Contracts.DeliveryPersons.Availabilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Gravy.Presentation.Helpers;

namespace Gravy.Presentation.Controllers;

[Authorize]
[Route("api/delivery-persons")]
public sealed class DeliveryPersonsController(ISender sender) : ApiController(sender)
{
    #region User claims

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    #endregion

    #region Get

    [HttpGet("all")]
    public async Task<IActionResult> GetAllDeliveryPersons(CancellationToken cancellationToken)
    {
        var query = new GetAllDeliveryPersonsQuery();
        var response = await Sender.Send(query, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    #endregion

    #region Availabilities

    #region Get

    [HttpGet("availabilities")]
    public async Task<IActionResult> GetAvailabilities(
        CancellationToken cancellationToken)
    {
        var query = new GetDeliveryPersonAvailabilitiesQuery(GetUserId());
        var response = await Sender.Send(query, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    #endregion

    #region Create/Update/Delete  (Receiving Datetime example : "startTimeUtc": "2024-12-17T10:43:00Z")

    [HttpPost("availabilities")]
    public async Task<IActionResult> CreateAvailability(
        [FromBody] CreateAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddAvailabilityCommand(
            GetUserId(),
            request.StartTime.ToUtc(),
            request.EndTime.ToUtc());

        var result = await Sender.Send(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    [HttpPut("availabilities/{availabilityId:guid}")]
    public async Task<IActionResult> UpdateAvailability(
        Guid availabilityId,
        [FromBody] UpdateAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAvailabilityCommand(
            GetUserId(),
            availabilityId,
            request.StartTime.ToUtc(),
            request.EndTime.ToUtc());

        var result = await Sender.Send(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    [HttpDelete("availabilities/{availabilityId:guid}")]
    public async Task<IActionResult> DeleteAvailability(
        Guid availabilityId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteAvailabilityCommand(
            GetUserId(),
            availabilityId);

        var result = await Sender.Send(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    #endregion

    #endregion
}