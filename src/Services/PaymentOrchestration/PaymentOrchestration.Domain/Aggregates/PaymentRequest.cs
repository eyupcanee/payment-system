using PaymentOrchestration.Domain.Common.Abstract;
using PaymentOrchestration.Domain.Common.Concrete;
using PaymentOrchestration.Domain.Enums;
using PaymentOrchestration.Domain.Errors;
using PaymentOrchestration.Domain.Events;
using PaymentOrchestration.Domain.ValueObjects;

namespace PaymentOrchestration.Domain.Aggregates;

public class PaymentRequest : AggregateRoot
{
    public Guid Id { get; private set; }
    public Money Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string CardHolderName { get; private set; }
    public string TokenizedCardNumber { get; private set; }
    public DateTime CreatedDate { get; private set; }
    
    private PaymentRequest() {}

    public static PaymentRequest Create(Money amount, string cardHolderName, string tokenizedCardNumber)
    {
        if (amount.Amount <= 0)
        {
            throw new ArgumentException(DomainErrorKeys.Payment.NegativeAmount, nameof(amount));
        }

        return new PaymentRequest()
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            CardHolderName = cardHolderName,
            TokenizedCardNumber = tokenizedCardNumber,
            Status = PaymentStatus.Pending,
            CreatedDate = DateTime.UtcNow,
        };
    }

    public void MarkAsProcessing()
    {
        if (Status == PaymentStatus.Pending)
        {
            Status = PaymentStatus.Processing;
        }
    }

    public void MarkAsCompleted()
    {
        Status = PaymentStatus.Completed;
    }

    public void MarkAsFailed(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException(DomainErrorKeys.Payment.InvalidReason, nameof(reason));       
        }
        
        Status = PaymentStatus.Failed;
        AddDomainEvent(new PaymentFailedEvent(Id, reason));
    }

    public void MarkAsCancelled()
    {
        Status = PaymentStatus.Cancelled;  
    }
}