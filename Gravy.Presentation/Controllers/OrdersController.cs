using Gravy.Application.Orders.Commands.CreateOrder;
using Gravy.Application.Orders.Commands.Deliviries.AssignDelivery;
using Gravy.Application.Orders.Commands.Deliviries.CompleteDelivery;
using Gravy.Application.Orders.Commands.Deliviries.CreateDelivery;
using Gravy.Application.Orders.Commands.OrderItems.AddOrderItem;
using Gravy.Application.Orders.Commands.Payments.CompletePayment;
using Gravy.Application.Orders.Commands.Payments.SetPayment;
using Gravy.Application.Orders.Queries.GetOrderById;
using Gravy.Domain.Shared;
using Gravy.Presentation.Abstractions;
using Gravy.Presentation.Contracts.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gravy.Presentation.Controllers;

[Authorize]
[Route("api/orders")]
public sealed class OrdersController(ISender sender) : ApiController(sender)
{
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    #region Get
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderById(Guid id, 
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);
        Result<OrderResponse> response = await Sender.Send(query, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }
    #endregion

    #region Order-Item related
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
    #endregion

    #region Workflow for order delivery
    // 1. Pending: Order created but awaiting payment.
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

    // 2. Confirmed: Payment is confirmed, delivery can be initiated.
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

    // 3. Delivery Pending: Delivery is created but not yet assigned.
    [HttpPost("{orderId:guid}/delivery")]
    public async Task<IActionResult> CreateDelivery(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var command = new CreateDeliveryCommand(orderId);
        Result<Guid> result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result.Value);
    }

    // 4. Delivery Assigned: Delivery is assigned to a delivery person.
    [HttpPut("{orderId:guid}/assign-delivery")]
    public async Task<IActionResult> AssignDelivery(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var command = new AssignDeliveryCommand(orderId);
        Result result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }

    // 5. In Progress: Delivery is picked up by the delivery person.
    // get status or another endpoint

    // 6. Completed: Delivery completed
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

    // 7. Payment finalized.
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
    #endregion
}
