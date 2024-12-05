using Gravy.Application.Users.Queries.GetAllDeliveryPersons;
using Gravy.Domain.Shared;
using Gravy.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gravy.Presentation.Controllers;

[Route("api/deliverypersons")]
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
}