namespace Common.Contracts.Events.Payment;

public record PaymentRequestSubmittedEvent
{
    public Guid PaymentRequestId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
}