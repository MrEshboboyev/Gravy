using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.UpdateAvailability;

internal sealed class UpdateAvailabilityCommandHandler(
    IUserRepository userRepository,
    IDeliveryPersonAvailabilityRepository deliveryPersonAvailabilityRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateAvailabilityCommand>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IDeliveryPersonAvailabilityRepository _deliveryPersonAvailabilityRepository =
        deliveryPersonAvailabilityRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateAvailabilityCommand request,
        CancellationToken cancellationToken)
    {
        var (userId, availabilityId, startTimeUtc, endTimeUtc) = request;

        #region Get User with Delivery Person Details (and availabilities)

        var user = await _userRepository.GetByIdWithDeliveryPersonDetailsAsync(
            userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(
                DomainErrors.User.NotFound(userId));
        }

        #endregion

        #region Checking Delivery Person details exist for this user

        if (user.DeliveryPersonDetails is null)
        {
            return Result.Failure(
                DomainErrors.User.DeliveryPersonDetailsNotExist(userId));
        }

        #endregion

        #region Update Delivery Person Availabity in this user delivery person details

        var updateAvailabilityResult = user.UpdateDeliveryPersonAvailability(
            availabilityId,
            startTimeUtc,
            endTimeUtc);
        if (updateAvailabilityResult.IsFailure)
        {
            return Result.Failure(
                updateAvailabilityResult.Error);
        }

        #endregion

        #region Update database

        _deliveryPersonAvailabilityRepository.Update(updateAvailabilityResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        #endregion

        return Result.Success();
    }
}