namespace Common.Contracts.Commands.Fraud;

public record CheckFraudCommand
{
    public Guid PaymentRequestId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; }
}