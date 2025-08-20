using Asp.Versioning;
using Common.Authorization;
using Common.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    [Authorize(Policy = "payment:write")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequestDto request)
    {
        var command = new CreatePaymentRequestCommand(request.Amount, request.Currency, request.CardHolderName, request.TokenizedCardNumber);
        
        var paymentId = await _mediator.Send(command);
        
        var response = ApiResponse<object>.SuccessResponse(paymentId,200,"Created Payment Request");
            
        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "payment:read")]
    public async Task<IActionResult> GetPaymentById(Guid id)
    {
        var query = new GetPaymentRequestByIdQuery(id);

        var result = await _mediator.Send(query);


        return result is not null
            ? Ok(ApiResponse<object>.SuccessResponse(result, 200, "Payment Request Found"))
            : Ok("Bulunamadu");
    }
}