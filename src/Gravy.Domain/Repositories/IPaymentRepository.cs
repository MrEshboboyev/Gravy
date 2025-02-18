using Gravy.Domain.Entities;

namespace Gravy.Domain.Repositories;

public interface IPaymentRepository
{
    void Add(Payment payment);
    void Update(Payment payment);
}

