using FluentValidation;

namespace Gravy.Application.Users.Commands.Customers.AddCustomerDetails;

internal sealed class AddCustomerDetailsCommandValidator : AbstractValidator<AddCustomerDetailsCommand>
{
    public AddCustomerDetailsCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();

        RuleFor(c => c.Street).NotEmpty();

        RuleFor(c => c.City).NotEmpty();

        RuleFor(c => c.State).NotEmpty();

        RuleFor(c => c.Latitude).NotEmpty();

        RuleFor(c => c.Longitude).NotEmpty();
    }
}

