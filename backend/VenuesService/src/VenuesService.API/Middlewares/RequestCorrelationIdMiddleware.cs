using Serilog.Context;

namespace VenuesService.API.Middlewares;

public class RequestCorrelationIdMiddleware
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";
    private const string CorrelationIdPropertyName = "CorrelationId";
    
    private readonly RequestDelegate _next;

    public RequestCorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationIdValues);
        var correlationId = correlationIdValues.FirstOrDefault() ?? context.TraceIdentifier;

        using (LogContext.PushProperty(CorrelationIdPropertyName, correlationId))
        {
            await _next(context);
        }
    }
}

public static class RequestCorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestCorrelationId(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestCorrelationIdMiddleware>();
    }
}