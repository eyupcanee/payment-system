namespace Common.Contracts.Commands.Payment;

public record ProcessPaymentWithBankCCommand
{
    public Guid PaymentRequestId { get; init; }
    public decimal Amount { get; init; }
    public string TokenizedCardNumber { get; init; }
}