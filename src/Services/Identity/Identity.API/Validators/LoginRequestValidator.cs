using Common.Constants;
using FluentValidation;
using Identity.API.DTOs;
using Microsoft.Extensions.Localization;

namespace Identity.API.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator(IStringLocalizer<RegisterRequestValidator> localizer)
    {
        RuleFor(x =>x.Email)
            .NotEmpty()
            .WithErrorCode(ErrorCodes.RequiredField)
            .WithMessage(localizer["Email_Required"]);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithErrorCode(ErrorCodes.RequiredField)
            .WithMessage(localizer["Password_Required"])
            .MinimumLength(6)
            .WithMessage(localizer["Password_MinimumLength"])
            .WithErrorCode(ErrorCodes.MinimumLength);
    }
}