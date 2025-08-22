namespace PaymentOrchestration.Application.Configuration.Bank;

public class BankConfig
{
    public string Name { get; set; }
    public double CommissionRate { get; set; }
    public double MinimumFee { get; set; }
    public List<string> SupportedCardSchemes { get; set; } = new();
}