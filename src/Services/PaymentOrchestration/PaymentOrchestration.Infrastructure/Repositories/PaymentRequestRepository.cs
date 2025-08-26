using Microsoft.EntityFrameworkCore;
using PaymentOrchestration.Application.Interfaces.Repositories;
using PaymentOrchestration.Domain.Aggregates;

namespace PaymentOrchestration.Infrastructure.Repositories;

public class PaymentRequestRepository : IPaymentRequestRepository
{
    private readonly PaymentDbContext _context;

    public PaymentRequestRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PaymentRequest paymentRequest)
    {
        await _context.PaymentRequests.AddAsync(paymentRequest);
    }

    public async Task<PaymentRequest?> GetByIdAsync(Guid id)
    {
        return await _context.PaymentRequests.FirstOrDefaultAsync(pr => pr.Id == id);
    }
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}