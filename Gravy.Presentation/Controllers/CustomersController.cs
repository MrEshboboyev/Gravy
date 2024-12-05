using Gravy.Application.Users.Queries.GetAllCustomers;
using Gravy.Domain.Shared;
using Gravy.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gravy.Presentation.Controllers;

[Route("api/customers")]
public sealed class CustomersController(ISender sender) : ApiController(sender)
{
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet("all")]
    public async Task<IActionResult> GetAllCustomers(CancellationToken cancellationToken)
    {
        var query = new GetAllCustomersQuery();
        Result<CustomerListResponse> response = await Sender.Send(query, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }
}