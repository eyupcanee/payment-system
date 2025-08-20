using MediatR;
using PaymentOrchestration.Application.Features.DTOs;
using PaymentOrchestration.Application.Interfaces.Repositories;

namespace PaymentOrchestration.Application.Features.Queries.GetPaymentRequestById;

public class GetPaymentRequestByIdQueryHandler : IRequestHandler<GetPaymentRequestByIdQuery, PaymentRequestDto?>
{
    private readonly IPaymentRequestRepository _paymentRequestRepository;
    
    public GetPaymentRequestByIdQueryHandler(IPaymentRequestRepository paymentRequestRepository)
    {
        _paymentRequestRepository = paymentRequestRepository;
    }

    public async Task<PaymentRequestDto?> Handle(GetPaymentRequestByIdQuery request,
        CancellationToken cancellationToken)
    {
        var paymentRequest = await _paymentRequestRepository.GetByIdAsync(request.PaymentId);

        if (paymentRequest is null)
        {
            return null;
        }

        return new PaymentRequestDto(
            paymentRequest.Id,
            paymentRequest.Amount.Amount,
            paymentRequest.Amount.Currency,
            paymentRequest.Status.ToString(),
            paymentRequest.CardHolderName,
            paymentRequest.CreatedDate
        );
    }
}