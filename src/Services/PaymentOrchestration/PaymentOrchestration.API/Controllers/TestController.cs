using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PaymentOrchestration.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet("test")]
    public IActionResult Test()
    {
        var headers = Request.Headers;
        foreach (var header in headers)
        {
            Console.WriteLine($"{header.Key} = {header.Value}");
        }
        return Ok("Headerler loglandı");
    }


    [HttpGet("headers")]
    public IActionResult GetHeaders()
    {
        var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
        return Ok(new { Headers = headers });
    }

    [HttpGet("claims")]
    public IActionResult GetClaims()
    {
        var claims = User.Claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList();
        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
        
        return Ok(new 
        { 
            IsAuthenticated = isAuthenticated,
            Claims = claims,
            AuthenticationType = User.Identity?.AuthenticationType
        });
    }

    [HttpGet("permissions")]
    [Authorize]
    public IActionResult GetPermissions()
    {
        var permissions = User.Claims
            .Where(c => c.Type == "permissions")
            .Select(c => c.Value)
            .ToList();

        return Ok(new { Permissions = permissions });
    }

    [HttpGet("payment-read")]
    [Authorize(Policy = "payment:read")]
    public IActionResult TestPaymentRead()
    {
        return Ok(new { Message = "Payment read permission başarılı!", UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value });
    }

    [HttpGet("payment-write")]
    [Authorize(Policy = "test:write")]
    public IActionResult TestPaymentWrite()
    {
        return Ok(new { Message = "Payment write permission başarılı!", UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value });
    }
}
