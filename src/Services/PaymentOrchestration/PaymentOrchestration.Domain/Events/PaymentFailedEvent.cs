using PaymentOrchestration.Domain.Common.Abstract;

namespace PaymentOrchestration.Domain.Events;

public record PaymentFailedEvent(Guid PaymentRequestId, string Reason) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}