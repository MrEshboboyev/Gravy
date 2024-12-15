using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.DeleteAvailability;

internal sealed class DeleteAvailabilityCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteAvailabilityCommand>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeleteAvailabilityCommand request,
        CancellationToken cancellationToken)
    {
        var (userId, availabilityId) = request;

        #region Get User with Delivery Person Details (and availabilities)

        var user = await _userRepository.GetByIdWithDeliveryPersonDetailsAsync(
            userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(
                DomainErrors.User.NotFound(userId));
        }

        #endregion

        #region Delete Delivery Person Availabity from this user delivery person details

        var deleteAvailabilityResult = user.DeleteDeliveryPersonAvailability(
            availabilityId);
        if (deleteAvailabilityResult.IsFailure)
        {
            return Result.Failure(
                deleteAvailabilityResult.Error);
        }

        #endregion

        #region Update database

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        #endregion

        return Result.Success();
    }
}