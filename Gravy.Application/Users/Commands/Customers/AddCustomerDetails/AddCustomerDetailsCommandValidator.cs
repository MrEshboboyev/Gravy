using FluentValidation;

namespace Gravy.Application.Users.Commands.Customers.AddCustomerDetails;

public sealed class AddCustomerDetailsCommandValidator : AbstractValidator<AddCustomerDetailsCommand>
{
    public AddCustomerDetailsCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();

        RuleFor(c => c.Street).NotEmpty();

        RuleFor(c => c.City).NotEmpty();

        RuleFor(c => c.State).NotEmpty();

        RuleFor(c => c.PostalCode).NotEmpty();
    }
}

