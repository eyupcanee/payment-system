namespace PaymentOrchestration.Application.Features.DTOs;

public record PaymentRequestDto(
    Guid Id,
    decimal Amount,
    string Currency,
    string Status,
    string CardHolderName,
    DateTime CreatedDate
    );