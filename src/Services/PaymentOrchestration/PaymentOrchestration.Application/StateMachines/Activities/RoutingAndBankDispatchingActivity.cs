using Common.Contracts.Commands.Payment;
using Common.Contracts.Events.Fraud;
using MassTransit;
using Microsoft.Extensions.Logging;
using PaymentOrchestration.Application.Services.Abstract;
using PaymentOrchestration.Domain.Aggregates;

namespace PaymentOrchestration.Application.StateMachines.Activities;

public class RoutingAndBankDispatchingActivity : IStateMachineActivity<PaymentRequestSagaState,FraudCheckPassedEvent>
{
    private readonly IRoutingService _routingService;
    private readonly ILogger<RoutingAndBankDispatchingActivity> _logger;

    public RoutingAndBankDispatchingActivity(IRoutingService routingService,
        ILogger<RoutingAndBankDispatchingActivity> logger)
    {
        _routingService = routingService;
        _logger = logger;
    }
    
    public void Probe(ProbeContext context) => context.CreateScope("routing-and-dispatch");
    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);
    public async Task Execute(BehaviorContext<PaymentRequestSagaState, FraudCheckPassedEvent> context, IBehavior<PaymentRequestSagaState, FraudCheckPassedEvent> next)
    {
        var targetBank = _routingService.GetTargetBankForSaga(context.Saga.TokenizedCardNumber,context.Saga.Amount);
        _logger.LogInformation("Routing decision for Payment ID {CorrelationId}: Target bank is {TargetBank}", context.Saga.CorrelationId, targetBank);
        
        if (targetBank == "BankA")
        {
            await context.Publish(new ProcessPaymentWithBankACommand
            {
                PaymentRequestId = context.Saga.CorrelationId,
                Amount = context.Saga.Amount,
                TokenizedCardNumber = context.Saga.TokenizedCardNumber
            });
        }
        else if (targetBank == "BankB")
        {
            await context.Publish(new ProcessPaymentWithBankBCommand
            {
                PaymentRequestId = context.Saga.CorrelationId,
                Amount = context.Saga.Amount,
                TokenizedCardNumber = context.Saga.TokenizedCardNumber
            });
        }
        
        await next.Execute(context);
    }
    
    public Task Faulted<TException>(BehaviorExceptionContext<PaymentRequestSagaState, FraudCheckPassedEvent, TException> context, IBehavior<PaymentRequestSagaState, FraudCheckPassedEvent> next) where TException : Exception
    {
        return next.Faulted(context);
    }
}