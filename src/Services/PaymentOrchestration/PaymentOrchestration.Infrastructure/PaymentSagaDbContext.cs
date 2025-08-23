using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using PaymentOrchestration.Infrastructure.Persistence.SagaStateMaps;

namespace PaymentOrchestration.Infrastructure;

public class PaymentSagaDbContext : SagaDbContext
{
    public PaymentSagaDbContext(DbContextOptions<PaymentSagaDbContext> options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new PaymentRequestSagaStateMap(); }
    }
}