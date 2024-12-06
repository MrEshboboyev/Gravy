using FluentValidation;

namespace Gravy.Application.Orders.Commands.Deliviries.AssignDelivery;

public sealed class AssignDeliveryCommandValidator : AbstractValidator<AssignDeliveryCommand>
{
    public AssignDeliveryCommandValidator()
    {
        RuleFor(delivery => delivery.OrderId).NotEmpty();

        RuleFor(delivery => delivery.DeliveryPersonId).NotEmpty();

        RuleFor(delivery => delivery.EstimatedDeliveryTime).NotEmpty();
    }
}

