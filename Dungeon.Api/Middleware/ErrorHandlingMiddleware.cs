using Dungeon.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace Dungeon.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var result = exception switch
        {
            MapNotFoundException => new { message = exception.Message, statusCode = HttpStatusCode.NotFound },
            InvalidMapException => new { message = exception.Message, statusCode = HttpStatusCode.BadRequest },
            ArgumentException => new { message = exception.Message, statusCode = HttpStatusCode.BadRequest },
            _ => new { message = "An internal server error occurred.", statusCode = HttpStatusCode.InternalServerError }
        };

        response.StatusCode = (int)result.statusCode;

        var jsonResponse = JsonSerializer.Serialize(new { error = result.message });
        await response.WriteAsync(jsonResponse);
    }
}