namespace Common.Contracts.Events.Payment;

public record PaymentProcessedEvent
{
    public Guid PaymentRequestId { get; init; }
    public string TransactionId { get; init; }
}