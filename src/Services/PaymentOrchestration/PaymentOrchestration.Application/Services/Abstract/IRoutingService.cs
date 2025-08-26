using PaymentOrchestration.Domain.Aggregates;

namespace PaymentOrchestration.Application.Services.Abstract;

public interface IRoutingService
{
    string GetTargetBank(PaymentRequest paymentRequest);
    string GetTargetBankForSaga(string tokenizedCardNumber,decimal amount);
}