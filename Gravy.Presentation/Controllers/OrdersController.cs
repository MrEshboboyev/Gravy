using Gravy.Application.Orders.Commands.AddOrderItem;
using Gravy.Application.Orders.Commands.AssignDelivery;
using Gravy.Application.Orders.Commands.CompleteDelivery;
using Gravy.Application.Orders.Commands.CompletePayment;
using Gravy.Application.Orders.Commands.CreateOrder;
using Gravy.Application.Orders.Commands.SetPayment;
using Gravy.Application.Orders.Queries.GetOrderById;
using Gravy.Domain.Shared;
using Gravy.Presentation.Abstractions;
using Gravy.Presentation.Contracts.Orders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gravy.Presentation.Controllers;

[Route("api/orders")]
public sealed class OrdersController(ISender sender) : ApiController(sender)
{
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);
        Result<OrderResponse> response = await Sender.Send(query, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(
            GetUserId(),
            request.RestaurantId,
            request.Street,
            request.City,
            request.State,
            request.PostalCode);

        Result<Guid> result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return CreatedAtAction(
           nameof(GetOrderById),
           new { id = result.Value },
           result.Value);
    }

    [HttpPost("{orderId:guid}/items")]
    public async Task<IActionResult> AddOrderItem(
        Guid orderId,
        [FromBody] AddOrderItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddOrderItemCommand(
            orderId,
            request.MenuItemId,
            request.Quantity, 
            request.Price);

        Result result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }

    [HttpPut("{orderId:guid}/delivery")]
    public async Task<IActionResult> AssignDelivery(
        Guid orderId,
        [FromBody] AssignDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AssignDeliveryCommand(
            orderId,
            request.DeliveryPersonId, 
            request.EstimatedDeliveryTime);

        Result result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }

    [HttpPut("{orderId:guid}/complete-delivery")]
    public async Task<IActionResult> CompleteDelivery(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var command = new CompleteDeliveryCommand(orderId);
        Result result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }

    [HttpPut("{orderId:guid}/payment")]
    public async Task<IActionResult> SetPayment(
        Guid orderId,
        [FromBody] SetPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SetPaymentCommand(
            orderId,
            request.Amount,
            request.PaymentMethod,
            request.TransactionId);

        Result result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }

    [HttpPut("{orderId:guid}/complete-payment")]
    public async Task<IActionResult> CompletePayment(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var command = new CompletePaymentCommand(orderId);
        Result result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }
}
