using Common.Contracts.Events.Payment;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace PaymentOrchestration.Application.StateMachines;

public class PaymentRequestSagaStateMachine : MassTransitStateMachine<PaymentRequestSagaState>
{
    private readonly ILogger<PaymentRequestSagaStateMachine> _logger;
    public State Submitted { get; private set; }
    public State FraudCheckPending { get; private set; }
    public State Completed { get; private set; }
    public State Failed { get; private set; }
    
    public Event<PaymentRequestSubmittedEvent> PaymentRequestSubmitted { get; private set; }

    public PaymentRequestSagaStateMachine(ILogger<PaymentRequestSagaStateMachine> logger)
    {
        _logger = logger;
        
        InstanceState(x => x.CurrentState);
        
        Event(() => PaymentRequestSubmitted, x => x.CorrelateById(context => context.Message.PaymentRequestId));
        
        Initially(
            When(PaymentRequestSubmitted)
                .Then(context =>
                {
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.Currency = context.Message.Currency;
                    _logger.LogWarning("SAGA Started for Payment ID: {PaymentId}", context.Message.PaymentRequestId);
                }).TransitionTo(Submitted)
                .Then(context =>
                {
                    _logger.LogWarning("Publishing CheckFraudCommand for Payment ID: {CorrelationId}", context.Saga.CorrelationId); 
                })
            );
        
    }
    
}