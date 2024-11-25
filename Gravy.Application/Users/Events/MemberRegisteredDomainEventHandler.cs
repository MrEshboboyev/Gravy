using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Entities;
using Gravy.Domain.Events;
using Gravy.Domain.Repositories;

namespace Gravy.Application.Users.Events;

internal sealed class UserRegisteredDomainEventHandler(
    IUserRepository userRepository)
          : IDomainEventHandler<UserRegisteredDomainEvent>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task Handle(
        UserRegisteredDomainEvent notification,
        CancellationToken cancellationToken)
    {
        User user = await _userRepository.GetByIdAsync(
            notification.UserId,
            cancellationToken);

        if (user is null)
        {
            return;
        }
    }
}
