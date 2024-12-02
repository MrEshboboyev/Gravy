using Gravy.Domain.Entities;

namespace Gravy.Domain.Repositories;

public interface ICustomerRepository
{
    void Add(Customer customer);
    void Update(Customer customer);
}