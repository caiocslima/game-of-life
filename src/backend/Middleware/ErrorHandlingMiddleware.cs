using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace GameOfLife.Middleware;

public class BoardNotFoundException : Exception
{
    public BoardNotFoundException(string message) : base(message) { }
}

public class NoFinalStateException : Exception
{
    public NoFinalStateException(string message) : base(message) { }
}

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        
        // Default to a 500 Internal Server Error
        var statusCode = StatusCodes.Status500InternalServerError;
        var title = "An unexpected error occurred.";
        var detail = exception.Message;

        switch (exception)
        {
            case BoardNotFoundException _:
                statusCode = StatusCodes.Status404NotFound;
                title = "Board Not Found";
                break;
            case NoFinalStateException _:
                statusCode = StatusCodes.Status422UnprocessableEntity;
                title = "Could Not Determine Final State";
                break;
        }

        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };
        
        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}