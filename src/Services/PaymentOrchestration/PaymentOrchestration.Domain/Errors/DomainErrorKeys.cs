namespace PaymentOrchestration.Domain.Errors;

public static class DomainErrorKeys
{
    public static class Money
    {
        public const string NegativeAmount = "Money_NegativeAmount";
        public const string InvalidCurrency = "Money_InvalidCurrency";
    }

    public static class Payment
    {
        public const string NegativeAmount = "Payment_NegativeAmount";
        public const string InvalidReason = "Payment_InvalidReason";
    }
}