using OrderService.Infrastructure.Middleware;

namespace OrderService.API.Extensions
{
    public static class LogCorrelationIdMiddlewareExtension
    {
        public static IApplicationBuilder UseLogCorrelationId(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LogCorrelationIdMiddleware>();
        }
    }
}
