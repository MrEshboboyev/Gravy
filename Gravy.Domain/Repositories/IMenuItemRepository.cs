using Gravy.Domain.Entities;

namespace Gravy.Domain.Repositories;

public interface IMenuItemRepository
{
    void Add(MenuItem menuItem);
}