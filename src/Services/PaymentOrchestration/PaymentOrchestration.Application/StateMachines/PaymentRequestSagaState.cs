using MassTransit;

namespace PaymentOrchestration.Application.StateMachines;

public class PaymentRequestSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; }
}