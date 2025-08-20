using MediatR;
using PaymentOrchestration.Application.Features.DTOs;

namespace PaymentOrchestration.Application.Features.Queries.GetPaymentRequestById;

public record GetPaymentRequestByIdQuery(Guid PaymentId) : IRequest<PaymentRequestDto?>;