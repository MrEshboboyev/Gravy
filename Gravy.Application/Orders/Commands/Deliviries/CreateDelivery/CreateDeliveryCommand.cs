using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Commands.Deliviries.CreateDelivery;

public sealed record CreateDeliveryCommand(
    Guid OrderId) : ICommand<Guid>;