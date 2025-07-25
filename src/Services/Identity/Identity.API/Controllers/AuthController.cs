using Asp.Versioning;
using Common.Contracts.Responses;
using Identity.API.DTOs;
using Identity.Application.Services.Abstract;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly IStringLocalizer<AuthController> _localizer;

    public AuthController(IIdentityService identityService, IStringLocalizer<AuthController> localizer)
    {
        _identityService = identityService;
        _localizer = localizer;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsyncV1([FromBody] LoginRequestDto request)
    {
        try
        {
            var token = await _identityService.LoginAsync(request.Email, request.Password);

            var tokenDto = new TokenDto(token);
            
            var response = ApiResponse<TokenDto>.SuccessResponse(tokenDto, 200,_localizer["Login_Success"]);
            
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            var response = ApiResponse<object>.FailResponse(_localizer["Invalid_Credentials"], 401);
            return Unauthorized(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse<object>.FailResponse(_localizer["Login_Failed"], 500);
            return BadRequest(response);
        }
    }
    
    
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsyncV1()
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            
            await _identityService.LogoutAsync(token);
            
            var response = ApiResponse<object>.SuccessResponse(null,200,_localizer["Logout_Success"]);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse<object>.FailResponse(_localizer["Logout_Failed"], 500);
            return BadRequest(response);
        }
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsyncV1([FromBody] RegisterRequestDto request)
    {
        try
        {
            var res =await _identityService.RegisterAsync(request.Email, request.Password);
            
            var response = ApiResponse<AppUser>.SuccessResponse(res, 201,_localizer["Register_Success"]);
            
            return StatusCode(201,response);
        }
        catch (ArgumentException ex)
        {
            var response = ApiResponse<object>.FailResponse(_localizer["Email_Exists"], 401);
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse<object>.FailResponse(_localizer["Register_Failed"], 500);
            return BadRequest(response);
        }
    }
}