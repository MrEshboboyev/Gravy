using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Users.Commands.Customers.AddCustomerDetails;

internal sealed class AddCustomerDetailsCommandHandler(
    IUserRepository userRepository,
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<AddCustomerDetailsCommand>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AddCustomerDetailsCommand request,
        CancellationToken cancellationToken)
    {
        var (userId, street, city, state, latitude, longitude) = request;

        #region Get User with Customer Details
        var user = await _userRepository.GetByIdWithCustomerDetailsAsync(userId, 
            cancellationToken);
        if (user is null)
        {
            return Result.Failure(
                DomainErrors.User.NotFound(userId));
        }
        #endregion

        #region Prepare Delivery Address
        Result<DeliveryAddress> deliveryAddressResult = DeliveryAddress.Create(
            street, 
            city,
            state, 
            latitude, 
            longitude);
        if (deliveryAddressResult.IsFailure)
        {
            return Result.Failure(
                deliveryAddressResult.Error);
        }
        #endregion

        #region Add Customer details to User
        var customerResult = user.AddCustomerDetails(deliveryAddressResult.Value);
        if (customerResult.IsFailure)
        {
            return Result.Failure(
                customerResult.Error);
        }
        #endregion

        #region Add and Update database
        _customerRepository.Add(customerResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        #endregion

        return Result.Success();
    }
}

