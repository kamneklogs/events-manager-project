using System.Net;
using System.Text.Json;
using events_manager_api.Common.Exceptions;

namespace events_manager_api.Common.ErrorHandler
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"exceptionType: {ex.GetType().Name}, message: {ex.Message}, env: {_env.EnvironmentName}");

                await HandleExceptionAsync(context, ex, _logger, _env);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<ErrorHandlerMiddleware> logger, IWebHostEnvironment env)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            var result = string.Empty;

            if (exception is WebApiException webApiException)
            {
                code = webApiException.StatusCode;

                var domainErrorPayload = new { errorMessage = webApiException.Message, errorData = webApiException.extraDataError };

                result = JsonSerializer.Serialize(domainErrorPayload);
            }
            else
            {
                result = JsonSerializer.Serialize(new { errorMessage = exception.Message });
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}