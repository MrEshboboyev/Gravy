using FluentValidation;

namespace Gravy.Application.Orders.Commands.Deliviries.AssignDelivery;

public sealed class AssignDeliveryCommandValidator : AbstractValidator<AssignDeliveryCommand>
{
    public AssignDeliveryCommandValidator()
    {
        RuleFor(delivery => delivery.OrderId).NotEmpty();
    }
}

