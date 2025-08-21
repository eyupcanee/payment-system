using Common.Contracts.Events.Payment;
using MediatR;
using PaymentOrchestration.Application.Interfaces.Repositories;
using PaymentOrchestration.Domain.ValueObjects;
using PaymentOrchestration.Domain.Aggregates;
using MassTransit;

namespace PaymentOrchestration.Application.Features.Commands.CreatePaymentRequest;

public class CreatePaymentRequestCommandHandler : IRequestHandler<CreatePaymentRequestCommand, Guid>
{
    private readonly IPaymentRequestRepository _paymentRequestRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreatePaymentRequestCommandHandler(IPaymentRequestRepository paymentRequestRepository, IPublishEndpoint publishEndpoint)
    {
        _paymentRequestRepository = paymentRequestRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> Handle(CreatePaymentRequestCommand request, CancellationToken cancellationToken)
    {
        var money = new Money(request.Amount, request.Currency);
        
        var newPaymentRequest = PaymentRequest.Create(money, request.CardHolderName, request.TokenizedCardNumber);

        await _paymentRequestRepository.AddAsync(newPaymentRequest);
        
        await _paymentRequestRepository.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new PaymentRequestSubmittedEvent
        {
            PaymentRequestId = newPaymentRequest.Id,
            Amount = newPaymentRequest.Amount.Amount,
            Currency = newPaymentRequest.Amount.Currency
        }, cancellationToken);
        
        return newPaymentRequest.Id;
    }
}