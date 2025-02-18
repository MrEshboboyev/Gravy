using Gravy.Domain.Entities;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Repositories;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken = default);
    Task<List<User>> GetUsersAsync(CancellationToken cancellationToken = default);
    void Add(User user);
    void Update(User user);

    #region Customer related
    Task<User> GetByIdWithCustomerDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    #endregion

    #region Delivery Person related
    Task<User> GetByIdWithDeliveryPersonDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    #endregion
}

