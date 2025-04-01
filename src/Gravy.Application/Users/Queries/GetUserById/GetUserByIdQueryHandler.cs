using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Users.Queries.GetUserById;

internal sealed class GetUserByIdQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        // Fetch the user from the repository
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);   

        // If user is not found, return a failure result
        if (user is null)
        {
            return Result.Failure<UserResponse>(
                DomainErrors.User.NotFound(request.UserId));
        }

        #region Prepare Response

        // Initialize response objects as null
        CustomerDetailsResponse customerDetailsResponse = null;
        DeliveryPersonDetailsResponse deliveryPersonDetailsResponse = null;

        // Populate CustomerDetailsResponse if CustomerDetails is not null
        if (user.CustomerDetails is not null)
        {
            var deliveryAddressObject = user.CustomerDetails.DefaultDeliveryAddress;
            string customerDefaultDeliveryAddress = $"{deliveryAddressObject.Street}/" +
                                                    $"{deliveryAddressObject.City}/" +
                                                    $"{deliveryAddressObject.State}";

            customerDetailsResponse = new CustomerDetailsResponse(
                user.CustomerDetails.Id,
                user.Id,
                customerDefaultDeliveryAddress,
                user.CustomerDetails.FavoriteRestaurants,
                user.CustomerDetails.CreatedOnUtc);
        }

        // Populate DeliveryPersonDetailsResponse if DeliveryPersonDetails is not null
        if (user.DeliveryPersonDetails is not null)
        {
            var vehicleObject = user.DeliveryPersonDetails.Vehicle;
            var deliveryPersonVehicle = $"Type : {vehicleObject.Type} | " +
                                         $"License Plate : {vehicleObject.LicensePlate}";

            deliveryPersonDetailsResponse = new DeliveryPersonDetailsResponse(
                user.DeliveryPersonDetails.Id,
                user.Id,
                deliveryPersonVehicle,
                user.DeliveryPersonDetails.AssignedDeliveries,
                user.DeliveryPersonDetails.CreatedOnUtc);
        }

        #endregion

        // Create and return the UserResponse object
        var response = new UserResponse(
            user.Id,
            user.Email.Value,
            user.FirstName.Value,
            user.LastName.Value,
            customerDetailsResponse,
            deliveryPersonDetailsResponse);

        return response;
    }
}
