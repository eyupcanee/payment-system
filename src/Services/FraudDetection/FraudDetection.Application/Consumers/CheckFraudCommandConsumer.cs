using Common.Contracts.Commands.Fraud;
using Common.Contracts.Events.Fraud;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FraudDetection.Application.Consumers;

public class CheckFraudCommandConsumer : IConsumer<CheckFraudCommand>
{
    private readonly ILogger<CheckFraudCommandConsumer> _logger;

    public CheckFraudCommandConsumer(ILogger<CheckFraudCommandConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CheckFraudCommand> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received fraud check command for Payment ID: {PaymentId}, Amount: {Amount}", 
            message.PaymentRequestId, message.Amount);

        if (message.Amount > 5000)
        {
            _logger.LogWarning("Fraud detected for Payment ID: {PaymentId}. Amount is too high.", message.PaymentRequestId);
            
            await context.Publish(new FraudCheckFailedEvent
            {
                PaymentRequestId = message.PaymentRequestId,
                Reason = "Amount exceeds the limit of 5000."
            });
        }
        else
        {
            _logger.LogInformation("Fraud check passed for Payment ID: {PaymentId}", message.PaymentRequestId);

            await context.Publish(new FraudCheckPassedEvent
            {
                PaymentRequestId = message.PaymentRequestId,
            });
        }
    }
}