using FluentValidation;

namespace Gravy.Application.Orders.Commands.Deliveries.AssignDelivery;

internal sealed class AssignDeliveryCommandValidator : AbstractValidator<AssignDeliveryCommand>
{
    public AssignDeliveryCommandValidator()
    {
        RuleFor(delivery => delivery.OrderId).NotEmpty();
    }
}

