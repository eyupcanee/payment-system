using PaymentOrchestration.Domain.Aggregates;

namespace PaymentOrchestration.Application.Interfaces.Repositories;

public interface IPaymentRequestRepository
{
    Task AddAsync(PaymentRequest paymentRequest);
    Task<PaymentRequest?> GetByIdAsync(Guid id);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}