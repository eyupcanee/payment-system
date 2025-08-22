namespace Common.Contracts.Events.Fraud;

public record FraudCheckFailedEvent
{
    public Guid PaymentRequestId { get; init; }
    public string Reason { get; init; }      
}