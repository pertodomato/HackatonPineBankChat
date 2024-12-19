//./src/PineBank.API/Middleware/ExceptionMiddleware.cs
namespace PineBank.API.Middleware
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System.Text.Json;

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try {
                await _next(context);
            } catch (Exception ex) {
                _logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            var resp = new { error = ex.Message, traceId = Activity.Current?.Id ?? context.TraceIdentifier };
            return context.Response.WriteAsync(JsonSerializer.Serialize(resp));
        }
    }
}
