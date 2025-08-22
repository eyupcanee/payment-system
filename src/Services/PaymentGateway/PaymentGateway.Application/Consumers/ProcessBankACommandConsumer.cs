using Common.Contracts.Commands.Payment;
using Common.Contracts.Events.Payment;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace PaymentGateway.Application.Consumers;

public class ProcessBankACommandConsumer : IConsumer<ProcessPaymentWithBankACommand>
{
    private readonly ILogger<ProcessBankACommandConsumer> _logger;
    
    public ProcessBankACommandConsumer(ILogger<ProcessBankACommandConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<ProcessPaymentWithBankACommand> context)
    {
        var transactionId = $"BANK_A_TRX_{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        _logger.LogInformation("[Bank A] Processing payment for {PaymentId}", context.Message.PaymentRequestId);
        await context.Publish(new PaymentProcessedEvent { PaymentRequestId = context.Message.PaymentRequestId, TransactionId = transactionId});
    }
}