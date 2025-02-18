using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Users.Commands.DeliveryPersons.AddDeliveryPersonDetails;

internal sealed class AddDeliveryPersonDetailsCommandHandler(
    IUserRepository userRepository,
    IDeliveryPersonRepository deliveryPersonRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<AddDeliveryPersonDetailsCommand>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IDeliveryPersonRepository _deliveryPersonRepository = deliveryPersonRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AddDeliveryPersonDetailsCommand request,
        CancellationToken cancellationToken)
    {
        var (userId, type, licensePlate, latitude, longitude) = request;

        #region Get User with Delivery Person Details
        var user = await _userRepository.GetByIdWithDeliveryPersonDetailsAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(
                DomainErrors.User.NotFound(userId));
        }
        #endregion

        #region Prepare Vehicle
        Result<Vehicle> vehicleResult = Vehicle.Create(type, licensePlate);
        if (vehicleResult.IsFailure)
        {
            return Result.Failure(
                vehicleResult.Error);
        }
        #endregion

        #region Prepare Location
        Result<Location> locationResult = Location.Create(latitude, longitude);
        if (locationResult.IsFailure)
        {
            return Result.Failure(
                locationResult.Error);
        }
        #endregion

        #region Add Delivery Person details to User
        var deliveryPersonResult = user.AddDeliveryPersonDetails(
            vehicleResult.Value,
            locationResult.Value);
        if (deliveryPersonResult.IsFailure)
        {
            return Result.Failure(
                deliveryPersonResult.Error);
        }
        #endregion

        #region Add and Update database
        _deliveryPersonRepository.Add(deliveryPersonResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        #endregion

        return Result.Success();
    }
}