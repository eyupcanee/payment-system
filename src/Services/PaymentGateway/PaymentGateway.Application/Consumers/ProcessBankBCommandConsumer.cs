using Common.Contracts.Commands.Payment;
using Common.Contracts.Events.Payment;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace PaymentGateway.Application.Consumers;

public class ProcessBankBCommandConsumer : IConsumer<ProcessPaymentWithBankBCommand>
{
    private readonly ILogger<ProcessBankBCommandConsumer> _logger;
    
    public ProcessBankBCommandConsumer(ILogger<ProcessBankBCommandConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProcessPaymentWithBankBCommand> context)
    {
        _logger.LogInformation("[Bank B] Processing payment for {PaymentId}", context.Message.PaymentRequestId);
        
        if (context.Message.Amount % 1.0m == 0.50m)
        {
            await context.Publish(new PaymentProcessingFailedEvent { PaymentRequestId = context.Message.PaymentRequestId, Reason = "Bank B declined." });
        }
        else
        {
            var transactionId = $"BANK_B_TRX_{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            await context.Publish(new PaymentProcessedEvent { PaymentRequestId = context.Message.PaymentRequestId, TransactionId = transactionId});
        }
    }
}