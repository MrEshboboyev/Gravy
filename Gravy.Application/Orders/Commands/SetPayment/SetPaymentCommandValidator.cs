using FluentValidation;

namespace Gravy.Application.Orders.Commands.SetPayment;

public sealed class SetPaymentCommandValidator : AbstractValidator<SetPaymentCommand>
{
    public SetPaymentCommandValidator()
    {
        RuleFor(payment => payment.OrderId).NotEmpty();

        RuleFor(payment => payment.Method).NotEmpty();
        
        RuleFor(payment => payment.Amount).NotEmpty();
        
        RuleFor(payment => payment.TransactionId).NotEmpty();
    }
}

