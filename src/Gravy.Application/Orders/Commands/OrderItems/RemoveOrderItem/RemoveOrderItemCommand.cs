using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Commands.OrderItems.RemoveOrderItem;

public sealed record RemoveOrderItemCommand(
    Guid OrderId,
    Guid OrderItemId) : ICommand;