using FluentAssertions;
using System.Net;
using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Infrastructure.Middleware;
using Xunit;

namespace Test.Infrastructure
{
    public class GlobalExceptionHandlingMiddlewareTests
    {
        [Fact]
        public async Task Should_Return_BadRequest_For_ValidationException()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var logger = Mock.Of<ILogger<GlobalExceptionHandlingMiddleware>>();
            var validationFailures = new[]
            {
            new ValidationFailure("Field1", "Error1"),
            new ValidationFailure("Field2", "Error2")
        };
            RequestDelegate next = _ => throw new ValidationException(validationFailures);

            var middleware = new GlobalExceptionHandlingMiddleware(next, logger);

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            context.Response.ContentType.Should().Be("application/problem+json");
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var response = await JsonDocument.ParseAsync(context.Response.Body);
            response.RootElement.GetProperty("title").GetString().Should().Be("One or more validation errors occurred.");
            response.RootElement.GetProperty("status").GetInt32().Should().Be((int)HttpStatusCode.BadRequest);
            response.RootElement.GetProperty("errors").EnumerateObject().Should().Contain(e => e.Name == "Field1");
        }

        [Fact]
        public async Task Should_Return_InternalServerError_For_UnhandledException()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var logger = Mock.Of<ILogger<GlobalExceptionHandlingMiddleware>>();
            RequestDelegate next = _ => throw new Exception("fail!");

            var middleware = new GlobalExceptionHandlingMiddleware(next, logger);

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            context.Response.ContentType.Should().Be("application/problem+json");
            context.Response.Body.Position = 0;
            var response = await JsonDocument.ParseAsync(context.Response.Body);
            response.RootElement.GetProperty("title").GetString().Should().Be("An unexpected error occurred.");
            response.RootElement.GetProperty("status").GetInt32().Should().Be((int)HttpStatusCode.InternalServerError);
            response.RootElement.GetProperty("detail").GetString().Should().Be("fail!");
        }

    }
}