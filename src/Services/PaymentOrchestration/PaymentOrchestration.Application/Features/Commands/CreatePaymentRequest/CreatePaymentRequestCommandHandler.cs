using MediatR;
using PaymentOrchestration.Application.Interfaces.Repositories;
using PaymentOrchestration.Domain.ValueObjects;
using PaymentOrchestration.Domain.Aggregates;

namespace PaymentOrchestration.Application.Features.Commands.CreatePaymentRequest;

public class CreatePaymentRequestCommandHandler : IRequestHandler<CreatePaymentRequestCommand, Guid>
{
    private readonly IPaymentRequestRepository _paymentRequestRepository;

    public CreatePaymentRequestCommandHandler(IPaymentRequestRepository paymentRequestRepository)
    {
        _paymentRequestRepository = paymentRequestRepository;
    }

    public async Task<Guid> Handle(CreatePaymentRequestCommand request, CancellationToken cancellationToken)
    {
        var money = new Money(request.Amount, request.Currency);
        
        var newPaymentRequest = PaymentRequest.Create(money, request.CardHolderName, request.TokenizedCardNumber);

        await _paymentRequestRepository.AddAsync(newPaymentRequest);
        
        return newPaymentRequest.Id;
    }
}