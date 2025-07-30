using MediatR;

namespace PaymentOrchestration.Domain.Common.Abstract;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}