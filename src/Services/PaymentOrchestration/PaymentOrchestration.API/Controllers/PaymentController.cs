using Asp.Versioning;
using Common.Authorization;
using Common.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using PaymentOrchestration.API.DTOs;
using PaymentOrchestration.Application.Features.Commands.CreatePaymentRequest;
using PaymentOrchestration.Application.Features.Queries.GetPaymentRequestById;

namespace PaymentOrchestration.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/payment")]
[ApiVersion("1.0")]
public class PaymentController: ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<PaymentController> _localizer;

    public PaymentController(IMediator mediator, IStringLocalizer<PaymentController> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    [HttpPost("create")]
    [Authorize(Policy = "payment:write")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequestDto request)
    {
        var command = new CreatePaymentRequestCommand(request.Amount, request.Currency, request.CardHolderName, request.TokenizedCardNumber);
        
        var paymentId = await _mediator.Send(command);
        
        var response = ApiResponse<object>.SuccessResponse(paymentId,200,_localizer["Payment_Created"]);
            
        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "payment:read")]
    public async Task<IActionResult> GetPaymentById(Guid id)
    {
        var query = new GetPaymentRequestByIdQuery(id);

        var result = await _mediator.Send(query);


        return result is not null
            ? Ok(ApiResponse<object>.SuccessResponse(result, 200, _localizer["Payment_Found"]))
            : NotFound(ApiResponse<object>.FailResponse(_localizer["Payment_NotFound"], 404));
    }
}