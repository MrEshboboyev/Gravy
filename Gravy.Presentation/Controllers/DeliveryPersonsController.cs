using Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.AddAvailability;
using Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.UpdateAvailability;
using Gravy.Application.Users.Queries.DeliveryPersons.GetAllDeliveryPersons;
using Gravy.Application.Users.Queries.DeliveryPersons.GetDeliveryPersonAvailabilities;
using Gravy.Domain.Shared;
using Gravy.Presentation.Abstractions;
using Gravy.Presentation.Contracts.DeliveryPersons.Availabilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gravy.Presentation.Controllers;

[Authorize]
[Route("api/delivery-persons")]
public sealed class DeliveryPersonsController(ISender sender) : ApiController(sender)
{
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet("all")]
    public async Task<IActionResult> GetAllDeliveryPersons(CancellationToken cancellationToken)
    {
        var query = new GetAllDeliveryPersonsQuery();
        Result<DeliveryPersonListResponse> response = await Sender.Send(query, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    #region Availabilities
    [HttpGet("availabilities")]
    public async Task<IActionResult> GetAvailabilities(
        CancellationToken cancellationToken)
    {
        var query = new GetDeliveryPersonAvailabilitiesQuery(GetUserId());
        Result<DeliveryPersonAvailabilityListResponse> response = await Sender.Send(query, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    [HttpPost("availabilities")]
    public async Task<IActionResult> CreateAvailiability(
        [FromBody] CreateAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddAvailabilityCommand(
            GetUserId(),
            request.StartTimeUtc,
            request.EndTimeUtc);

        Result result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }

    [HttpPut("availabilities/{availabilityId:guid}")]
    public async Task<IActionResult> UpdateAvailiability(
        Guid availabilityId,
        [FromBody] UpdateAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAvailabilityCommand(
            GetUserId(),
            availabilityId,
            request.StartTimeUtc,
            request.EndTimeUtc);

        Result result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }
    #endregion
}