﻿using Gravy.Application.Orders.Commands.CreateOrder;
using Gravy.Application.Orders.Commands.DeleteOrder;
using Gravy.Application.Orders.Commands.Deliveries.AssignDelivery;
using Gravy.Application.Orders.Commands.Deliveries.CompleteDelivery;
using Gravy.Application.Orders.Commands.Deliveries.CreateDelivery;
using Gravy.Application.Orders.Commands.OrderItems.AddOrderItem;
using Gravy.Application.Orders.Commands.OrderItems.RemoveOrderItem;
using Gravy.Application.Orders.Commands.OrderItems.UpdateOrderItem;
using Gravy.Application.Orders.Commands.Payments.CompletePayment;
using Gravy.Application.Orders.Commands.Payments.SetPayment;
using Gravy.Application.Orders.Queries.GetOrderById;
using Gravy.Application.Orders.Queries.GetOrdersByCustomer;
using Gravy.Presentation.Abstractions;
using Gravy.Presentation.Contracts.Orders;
using Gravy.Presentation.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gravy.Presentation.Controllers;

[Authorize]
[Route("api/orders")]
public sealed class OrdersController(ISender sender) : ApiController(sender)
{
    #region User Claims

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    #endregion

    #region Get

    [HttpGet]
    public async Task<IActionResult> GetCustomerOrders(CancellationToken cancellationToken)
    {
        var query = new GetOrdersByCustomerQuery(GetUserId());
    
        var response = await Sender.Send(query, cancellationToken);
        
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderById(Guid id, 
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);
        
        var response = await Sender.Send(query, cancellationToken);
        
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    #endregion

    #region Delete Order

    [HttpDelete("{orderId:guid}")]
    public async Task<IActionResult> DeleteOrder(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteOrderCommand(orderId);

        var result = await Sender.Send(command, cancellationToken);
        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    #endregion

    #region Order-Item related (Add/Update/Remove)

    [HttpPost("{orderId:guid}/items")]
    public async Task<IActionResult> AddOrderItem(
        Guid orderId,
        [FromBody] AddOrderItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddOrderItemCommand(
            orderId,
            request.MenuItemId,
            request.Quantity);

        var result = await Sender.Send(command, cancellationToken);
        
        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    [HttpPut("{orderId:guid}/items/{orderItemId:guid}")]
    public async Task<IActionResult> UpdateOrderItem(
        Guid orderId,
        Guid orderItemId,
        [FromBody] UpdateOrderItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateOrderItemCommand(
            orderId,
            orderItemId,
            request.Quantity);

        var result = await Sender.Send(command, cancellationToken);
        
        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    [HttpDelete("{orderId:guid}/items/{orderItemId:guid}")]
    public async Task<IActionResult> RemoveOrderItem(
        Guid orderId,
        Guid orderItemId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveOrderItemCommand(
            orderId,
            orderItemId);

        var result = await Sender.Send(command, cancellationToken);
        
        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    #endregion

    #region Workflow for order delivery

    // 1. Pending: Order created but awaiting payment.
    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        // Generate random location in Tashkent for testing if not provided
        var latitude = LocationHelpers.GetRandomLatitude();
        var longitude = LocationHelpers.GetRandomLongitude();

        var command = new CreateOrderCommand(
            GetUserId(),
            request.RestaurantId,
            request.Street,
            request.City,
            request.State,
            latitude,
            longitude);

        var result = await Sender.Send(command, cancellationToken);
        
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
            request.PaymentMethod,
            request.TransactionId);

        var result = await Sender.Send(command, cancellationToken);
        
        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    // 3. Delivery Pending: Delivery is created but not yet assigned.
    [HttpPost("{orderId:guid}/delivery")]
    public async Task<IActionResult> CreateDelivery(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var command = new CreateDeliveryCommand(orderId);

        var result = await Sender.Send(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result) : Ok(result.Value);
    }

    // 4. Delivery Assigned: Delivery is assigned to a delivery person.
    [HttpPut("{orderId:guid}/assign-delivery")]
    public async Task<IActionResult> AssignDelivery(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var command = new AssignDeliveryCommand(orderId);

        var result = await Sender.Send(command, cancellationToken);
        
        return result.IsFailure ? HandleFailure(result) : NoContent();
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
        
        var result = await Sender.Send(command, cancellationToken);
        
        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    // 7. Payment finalized.
    [HttpPut("{orderId:guid}/complete-payment")]
    public async Task<IActionResult> CompletePayment(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var command = new CompletePaymentCommand(orderId);
        
        var result = await Sender.Send(command, cancellationToken);
        
        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    #endregion
}
