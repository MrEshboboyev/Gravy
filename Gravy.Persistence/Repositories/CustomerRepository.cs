using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;

namespace Gravy.Persistence.Repositories;

public sealed class CustomerRepository(ApplicationDbContext dbContext) : ICustomerRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public void Add(Customer customer) =>
        _dbContext.Set<Customer>().Add(customer);

    public void Update(Customer customer) =>
        _dbContext.Set<Customer>().Update(customer);
}
