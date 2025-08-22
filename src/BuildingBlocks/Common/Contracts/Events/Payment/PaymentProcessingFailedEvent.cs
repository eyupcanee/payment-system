namespace Common.Contracts.Events.Payment;

public record PaymentProcessingFailedEvent
{
    public Guid PaymentRequestId { get; init; }
    public string Reason { get; init; }
}