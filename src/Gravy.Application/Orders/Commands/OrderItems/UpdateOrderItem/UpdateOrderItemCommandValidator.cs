﻿using FluentValidation;

namespace Gravy.Application.Orders.Commands.OrderItems.UpdateOrderItem;

internal sealed class UpdateOrderItemCommandValidator : AbstractValidator<UpdateOrderItemCommand>
{
    public UpdateOrderItemCommandValidator()
    {
        RuleFor(orderItem => orderItem.OrderId).NotEmpty();

        RuleFor(orderItem => orderItem.OrderItemId).NotEmpty();

        RuleFor(orderItem => orderItem.Quantity).GreaterThanOrEqualTo(1);
    }
}