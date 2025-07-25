using Common.Contracts.Responses;
using Common.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    public ValidationFilter(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var modelStateErrors = context.ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .SelectMany(kvp => kvp.Value.Errors.Select(e => new ErrorDetail
                {
                    Code = "MODEL_BINDING_ERROR",
                    Field = char.ToLowerInvariant(kvp.Key[0]) + kvp.Key.Substring(1),
                    Message = e.ErrorMessage
                })).ToList();
            
            var errorResponse = ApiResponse<object>.FailResponse(
                _localizer["InvalidModel"], 
                400, 
                modelStateErrors);
                
            context.Result = new BadRequestObjectResult(errorResponse);
            return;
        }
        
        var dto = context.ActionArguments.Values.FirstOrDefault();
        if (dto is null)
        {
            await next();
            return;
        }
        
        var validatorType = typeof(IValidator<>).MakeGenericType(dto.GetType());
        var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;
        if (validator is null)
        {
            await next();
            return;
        }
        
        var validationResult = await validator.ValidateAsync(new ValidationContext<object>(dto));
        
        if (!validationResult.IsValid)
        {
            var fluentErrors = validationResult.Errors.Select(err => new ErrorDetail
            {
                Code = err.ErrorCode,
                Field = char.ToLowerInvariant(err.PropertyName[0]) + err.PropertyName.Substring(1),
                Message = err.ErrorMessage
            }).ToList();
            
            var errorResponse = ApiResponse<object>.FailResponse(
                _localizer["ValidationFailed"], 
                400, 
                fluentErrors);
            
            context.Result = new BadRequestObjectResult(errorResponse);
            return;
        }

        await next();
    }
}