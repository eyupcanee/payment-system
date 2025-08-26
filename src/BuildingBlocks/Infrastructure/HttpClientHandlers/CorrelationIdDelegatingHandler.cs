using Microsoft.AspNetCore.Http;

namespace Infrastructure.HttpClientHandlers;

public class CorrelationIdDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string CorrelationIdHeaderName = "X-Correlation-ID";

    public CorrelationIdDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(CorrelationIdHeaderName,
                out var correlationId))
        {
            request.Headers.Add(CorrelationIdHeaderName, correlationId.ToString());;
        }
        
        return base.SendAsync(request, cancellationToken);
    }
}