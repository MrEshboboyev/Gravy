using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.AddAvailability;

internal sealed class AddAvailabilityCommandHandler(
    IUserRepository userRepository,
    IDeliveryPersonAvailabilityRepository deliveryPersonAvailabilityRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<AddAvailabilityCommand>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IDeliveryPersonAvailabilityRepository _deliveryPersonAvailabilityRepository =
        deliveryPersonAvailabilityRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AddAvailabilityCommand request,
        CancellationToken cancellationToken)
    {
        var (userId, startTimeUtc, endTimeUtc) = request;

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

        #region Add Delivery Person Availabity to this user delivery person details

        var addAvailabilityResult = user.AddDeliveryPersonAvailability(
            startTimeUtc,
            endTimeUtc);
        if (addAvailabilityResult.IsFailure)
        {
            return Result.Failure(
                addAvailabilityResult.Error);
        }

        #endregion

        #region Add and Update database

        _deliveryPersonAvailabilityRepository.Add(addAvailabilityResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        #endregion

        return Result.Success();
    }
}