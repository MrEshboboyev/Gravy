using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Commands.AddOrderItem;

public sealed record AddOrderItemCommand(
    Guid OrderId,
    Guid MenuItemId,
    int Quantity, 
    decimal Price) : ICommand;

