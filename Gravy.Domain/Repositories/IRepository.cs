using Gravy.Domain.Primitives;

namespace Gravy.Domain.Repositories;

public interface IRepository<T>
    where T : AggregateRoot
{
}