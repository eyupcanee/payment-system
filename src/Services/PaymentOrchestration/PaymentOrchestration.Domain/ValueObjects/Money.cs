using PaymentOrchestration.Domain.Errors;

namespace PaymentOrchestration.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new ArgumentException(DomainErrorKeys.Money.NegativeAmount,nameof(amount));
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException(DomainErrorKeys.Money.InvalidCurrency,nameof(currency));       
        }
        
        Amount = amount;
        Currency = currency;
    }
}