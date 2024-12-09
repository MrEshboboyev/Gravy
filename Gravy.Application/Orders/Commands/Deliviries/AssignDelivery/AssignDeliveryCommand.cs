﻿using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Commands.Deliviries.AssignDelivery;

public sealed record AssignDeliveryCommand(
    Guid OrderId) : ICommand;