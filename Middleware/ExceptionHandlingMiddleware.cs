using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync (HttpContext context)
    {
        try
        {
            // pass this request so the next middleware can see it
            await _next(context);
        }
        catch (Exception ex)
        {
            // if anyone down the line throws an error, it is caught here
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unexpected error occured.");
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Success = false,
            Message = "An error occured while processing your request."
        };

        switch (exception)
        {
            // if the exception is a validation exception, we can extract the errors
            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Validation errors occurred.";
                response.Errors = validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage});
                break;

            // if the exception is a key not found exception, we can extract the message
            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = "The requested resource was not found.";
                break;

            // if the exception is an unauthorized access exception, we can extract the message
            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = "You are not authorized to access this resource.";
                break;
                
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An internal server error occurred.";
                // in production do not expose exception details
                // response.Details = exception.Message; // for debugging
                break;
        }

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}

// simple helper class for response format

public class ErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public object? Errors { get; set; }
    public string? Details { get; set; }
}