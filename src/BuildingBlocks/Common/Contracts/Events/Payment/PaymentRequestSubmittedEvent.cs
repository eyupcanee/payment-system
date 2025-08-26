namespace Common.Contracts.Events.Payment;

public record PaymentRequestSubmittedEvent
{
    public Guid PaymentRequestId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }

    public string TokenizedCardNumber { get; set; }
    public string CardHolderName { get; set; }
    
}