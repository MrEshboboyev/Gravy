using FluentValidation;

namespace Gravy.Application.Orders.Commands.Deliveries.CreateDelivery;

internal sealed class CreateDeliveryCommandValidator : 
    AbstractValidator<CreateDeliveryCommand>
{
    public CreateDeliveryCommandValidator()
    {
        RuleFor(d => d.OrderId).NotEmpty();
    }
}

