using Common.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using PaymentOrchestration.API.DTOs;

namespace PaymentOrchestration.API.Validators;

public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequestDto>
{
    public CreatePaymentRequestValidator(IStringLocalizer<CreatePaymentRequestValidator> localizer)
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithErrorCode(ErrorCodes.GreaterThanZero).WithMessage(localizer["Amount_InvalidAmount"]);
        RuleFor(x => x.Currency)
            .NotEmpty().WithErrorCode(ErrorCodes.RequiredField).WithMessage(localizer["Currency_Required"]);
        RuleFor(x => x.CardHolderName)
            .NotEmpty().WithErrorCode(ErrorCodes.RequiredField).WithMessage(localizer["CardHolderName_Required"]);
        RuleFor(x => x.TokenizedCardNumber)
            .NotEmpty().WithErrorCode(ErrorCodes.RequiredField).WithMessage(localizer["TokenizedCardNumber_Required"]);
    }
}