using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentOrchestration.Application.Configuration.Bank;
using PaymentOrchestration.Application.Services.Abstract;
using PaymentOrchestration.Domain.Aggregates;

namespace PaymentOrchestration.Application.Services.Concrete;

public class RoutingService : IRoutingService
{
    private readonly List<BankConfig> _banks;
    private readonly ILogger<RoutingService> _logger;

    public RoutingService(IOptions<BankSettings> bankOptions, ILogger<RoutingService> logger)
    {
        _banks = bankOptions.Value.Banks;
        _logger = logger;
    }

    public string GetTargetBank(PaymentRequest paymentRequest)
    {
        var cardScheme = GetCardScheme(paymentRequest.TokenizedCardNumber);
        
        var eligibleBanks = _banks.Where(b => b.SupportedCardSchemes.Contains(cardScheme));

        if (!eligibleBanks.Any())
        {
            return _banks.FirstOrDefault()?.Name ?? "NoBankAvailable";
        }
        
        var costs = eligibleBanks.Select(bank => new
        {
            BankName = bank.Name,
            Cost = (double)paymentRequest.Amount.Amount * (bank.CommissionRate / 100.0) + bank.MinimumFee
        });
        
        var bestBank = costs.OrderBy(c => c.Cost).FirstOrDefault();

        return bestBank.BankName;
    }

    public string GetTargetBankForSaga(string tokenizedCardNumber, decimal amount)
    {
        var cardScheme = GetCardScheme(tokenizedCardNumber);
        _logger.LogWarning(cardScheme);
        _logger.LogWarning(_banks.Count.ToString());        
        var eligibleBanks = _banks.Where(b => b.SupportedCardSchemes.Contains(cardScheme));
        
        if (!eligibleBanks.Any())
        {
            return _banks.FirstOrDefault()?.Name ?? "NoBankAvailable";
        }
        
        var costs = eligibleBanks.Select(bank => new
        {
            BankName = bank.Name,
            Cost = (double)amount * (bank.CommissionRate / 100.0) + bank.MinimumFee
        });
        
        var bestBank = costs.OrderBy(c => c.Cost).FirstOrDefault();

        return bestBank.BankName;
    }
    
    
    private string GetCardScheme(string tokenizedCardNumber)
    {
        // Basit simülasyon: Token içinde kart tipi bilgisi olduğunu varsayıyoruz.
        if (tokenizedCardNumber.ToUpper().Contains("AMEX")) return "Amex";
        if (tokenizedCardNumber.ToUpper().Contains("VISA")) return "Visa";
        return "Mastercard"; 
    }
}