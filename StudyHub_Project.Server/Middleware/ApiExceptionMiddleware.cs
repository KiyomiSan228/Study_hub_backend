using StudyHub_Project.Server.Exceptions;

namespace StudyHub_Project.Server.Middleware;

public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(
        RequestDelegate next,
        ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException exception)
        {
            context.Response.Clear();

            context.Response.StatusCode =
                exception.StatusCode;

            context.Response.ContentType =
                "application/json";

            await context.Response.WriteAsJsonAsync(
                new
                {
                    message = exception.Message
                });
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Необработанная ошибка сервера.");

            context.Response.Clear();

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            context.Response.ContentType =
                "application/json";

            await context.Response.WriteAsJsonAsync(
                new
                {
                    message = "Внутренняя ошибка сервера."
                });
        }
    }
}