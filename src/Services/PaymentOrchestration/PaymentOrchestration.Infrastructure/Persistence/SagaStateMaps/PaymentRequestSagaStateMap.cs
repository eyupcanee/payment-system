using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentOrchestration.Application.StateMachines;

namespace PaymentOrchestration.Infrastructure.Persistence.SagaStateMaps;

public class PaymentRequestSagaStateMap : SagaClassMap<PaymentRequestSagaState>
{
    protected override void Configure(EntityTypeBuilder<PaymentRequestSagaState> entity, ModelBuilder model)
    {
        entity.ToTable("payment_request_saga_state"); 
        entity.HasKey(x => x.CorrelationId);
        entity.Property(x => x.CurrentState).HasMaxLength(256);
    }
}