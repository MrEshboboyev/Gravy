using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;

namespace Gravy.Persistence.Orders.Repositories.Payments;

public sealed class PaymentRepository(ApplicationDbContext dbContext) : IPaymentRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public void Add(Payment menuItem) =>
        _dbContext.Set<Payment>().Add(menuItem);

    public void Update(Payment menuItem) =>
        _dbContext.Set<Payment>().Update(menuItem);
}
