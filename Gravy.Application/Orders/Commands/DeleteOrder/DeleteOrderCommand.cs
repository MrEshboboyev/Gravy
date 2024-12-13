using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Commands.DeleteOrder;

public sealed record DeleteOrderCommand(
    Guid OrderId) : ICommand;