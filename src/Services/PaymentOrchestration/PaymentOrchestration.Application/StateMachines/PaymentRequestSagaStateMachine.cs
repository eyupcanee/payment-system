using Common.Contracts.Commands.Fraud;
using Common.Contracts.Events.Fraud;
using Common.Contracts.Events.Payment;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace PaymentOrchestration.Application.StateMachines;

public class PaymentRequestSagaStateMachine : MassTransitStateMachine<PaymentRequestSagaState>
{
    private readonly ILogger<PaymentRequestSagaStateMachine> _logger;
    public State Submitted { get; private set; }
    public State Completed { get; private set; }
    public State Failed { get; private set; }
    
    public Event<PaymentRequestSubmittedEvent> PaymentRequestSubmitted { get; private set; }
    public Event<FraudCheckPassedEvent> FraudCheckPassed { get; private set; }
    public Event<FraudCheckFailedEvent> FraudCheckFailed { get; private set; }

    public PaymentRequestSagaStateMachine(ILogger<PaymentRequestSagaStateMachine> logger)
    {
        _logger = logger;
        
        InstanceState(x => x.CurrentState);
        
        Event(() => PaymentRequestSubmitted, x => x.CorrelateById(context => context.Message.PaymentRequestId));
        Event(() => FraudCheckPassed, x => x.CorrelateById(context => context.Message.PaymentRequestId));
        Event(() => FraudCheckFailed, x => x.CorrelateById(context => context.Message.PaymentRequestId));
        Initially(
            When(PaymentRequestSubmitted)
                .Then(context =>
                {
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.Currency = context.Message.Currency;
                    _logger.LogWarning("SAGA Started for Payment ID: {PaymentId}", context.Message.PaymentRequestId);
                }).TransitionTo(Submitted).Publish(context => new CheckFraudCommand
                {
                    PaymentRequestId = context.Saga.CorrelationId,
                    Amount = context.Saga.Amount,
                    Currency = context.Saga.Currency
                })
            );
        
        During(Submitted,
            When(FraudCheckPassed)
                .Then(context => _logger.LogInformation("Fraud check PASSED for Payment ID: {CorrelationId}", context.Saga.CorrelationId))
                .TransitionTo(Completed)
                .Finalize(),
            When(FraudCheckFailed)
                .Then(context =>  _logger.LogError("Fraud check FAILED for Payment ID: {CorrelationId}. Reason: {Reason}", context.Saga.CorrelationId, context.Message.Reason))
                .TransitionTo(Failed)
                .Finalize());
        
        SetCompletedWhenFinalized();
        
    }
    
}