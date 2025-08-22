namespace Common.Contracts.Events.Fraud;

public record FraudCheckPassedEvent
{
    public Guid PaymentRequestId { get; init; }
}