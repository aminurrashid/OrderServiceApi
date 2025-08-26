using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace OrderService.Infrastructure.Middleware
{
    public class LogCorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public LogCorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.Headers.TryGetValue("Correlation-Id-Header", out StringValues correlationIds);
            var correlationId = correlationIds.FirstOrDefault() ?? Guid.NewGuid().ToString();

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(httpContext);
            }
        }
    }
}
