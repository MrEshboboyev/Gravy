using FluentValidation;

namespace Gravy.Application.Orders.Commands.Deliviries.CreateDelivery;

public sealed class CreateDeliveryCommandValidator : 
    AbstractValidator<CreateDeliveryCommand>
{
    public CreateDeliveryCommandValidator()
    {
        RuleFor(d => d.OrderId).NotEmpty();
    }
}

