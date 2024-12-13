using Gravy.Application.Abstractions.Messaging;
using Gravy.Application.Users.Queries.Common;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Users.Queries.DeliveryPersons.GetDeliveryPersonAvailabilities;

internal sealed class GetDeliveryPersonAvailabilitiesQueryHandler(
    IUserRepository userRepository)
    : IQueryHandler<GetDeliveryPersonAvailabilitiesQuery, DeliveryPersonAvailabilityListResponse>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<DeliveryPersonAvailabilityListResponse>> Handle(
        GetDeliveryPersonAvailabilitiesQuery request, CancellationToken cancellationToken)
    {
        #region Get User with delivery person details

        var user = await _userRepository.GetByIdWithDeliveryPersonDetailsAsync
            (request.UserId,
            cancellationToken);
        if (user is null)
        {
            return Result.Failure<DeliveryPersonAvailabilityListResponse>(
                DomainErrors.User.NotFound(request.UserId));
        }

        #endregion

        #region Checking Delivery Person details for this user

        if (user.DeliveryPersonDetails is null)
        {
            return Result.Failure<DeliveryPersonAvailabilityListResponse>(
                DomainErrors.User.DeliveryPersonDetailsNotExist(user.Id));
        }

        #endregion

        #region Prepare Response

        var availabilities = user
            .DeliveryPersonDetails.Availabilities;

        // Create and return the DeliveryPersonAvailabilityListResponse object
        var response = new DeliveryPersonAvailabilityListResponse(
            availabilities
                .Select(DeliveryPersonAvailabilityResponseFactory.Create)
                .ToList());

        #endregion

        return Result.Success(response);
    }
}
