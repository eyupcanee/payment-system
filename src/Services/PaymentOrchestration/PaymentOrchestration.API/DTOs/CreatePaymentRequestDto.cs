namespace PaymentOrchestration.API.DTOs;

public record CreatePaymentRequestDto(
    decimal Amount,
    string Currency,
    string CardHolderName,
    string TokenizedCardNumber
);