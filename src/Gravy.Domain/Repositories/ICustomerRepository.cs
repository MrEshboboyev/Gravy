using Gravy.Domain.Entities;

namespace Gravy.Domain.Repositories;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default); 
    void Add(Customer customer);
    void Update(Customer customer);
}