using Gravy.Domain.Entities;

namespace Gravy.Domain.Repositories;

public interface IMenuItemRepository
{
    Task<MenuItem> GetByIdAsync(Guid id, CancellationToken cancellationToken = default!);
    void Add(MenuItem menuItem);
    void Update(MenuItem menuItem);
}