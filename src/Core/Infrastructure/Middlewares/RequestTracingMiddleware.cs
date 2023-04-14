using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Core.Infrastructure;

public class RequestTracingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestTracingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        using (LogContext.PushProperty("TransactionId", context.TraceIdentifier))
        using (LogContext.PushProperty("Path", $"{context.Request?.Path.Value}{context.Request?.QueryString.Value}"))
        {
            await _next(context);
        }
    }
}