using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gravy.Persistence.Users.Repositories.Customers;

public sealed class CustomerRepository(ApplicationDbContext dbContext) : ICustomerRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<List<Customer>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
            await _dbContext
            .Set<Customer>()
            .ToListAsync(cancellationToken);

    public void Add(Customer customer) =>
        _dbContext.Set<Customer>().Add(customer);

    public void Update(Customer customer) =>
        _dbContext.Set<Customer>().Update(customer);
}
