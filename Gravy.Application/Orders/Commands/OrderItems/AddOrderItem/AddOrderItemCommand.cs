using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Commands.OrderItems.AddOrderItem;

public sealed record AddOrderItemCommand(
    Guid OrderId,
    Guid MenuItemId,
    int Quantity) : ICommand;

