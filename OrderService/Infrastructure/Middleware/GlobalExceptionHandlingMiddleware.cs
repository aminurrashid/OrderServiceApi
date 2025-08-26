using FluentValidation;
using System.Net;
using System.Text.Json;
using OrderService.Domain.Shared;

namespace OrderService.Infrastructure.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ValidationException vex)
            {
                _logger.LogWarning(vex, "Validation failed");
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                httpContext.Response.ContentType = "application/problem+json";

                var problem = new
                {
                    title = "One or more validation errors occurred.",
                    status = httpContext.Response.StatusCode,
                    errors = vex.Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        )
                };

                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }
            catch (DomainException de)
            {
                _logger.LogError(de, de.Message);
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var problem = new
                {
                    title = "Invalid request",
                    status = httpContext.Response.StatusCode,
                    detail = de.Message
                };

                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/problem+json";

                var problem = new
                {
                    title = "An unexpected error occurred.",
                    status = httpContext.Response.StatusCode,
                    detail = ex.Message
                };

                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }
        }
    }
}
