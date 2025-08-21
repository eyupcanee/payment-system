using MediatR;

namespace PaymentOrchestration.Application.Features.Commands.CreatePaymentRequest;

public record CreatePaymentRequestCommand(
    decimal Amount,
    string Currency,
    string CardHolderName,
    string TokenizedCardNumber) : IRequest<Guid>;