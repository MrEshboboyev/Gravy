using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Commands.OrderItems.UpdateOrderItem;

public sealed record UpdateOrderItemCommand(
    Guid OrderId,
    Guid OrderItemId,
    int Quantity) : ICommand;
