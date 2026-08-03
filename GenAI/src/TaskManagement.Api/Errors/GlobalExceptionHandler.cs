using Microsoft.AspNetCore.Diagnostics;
namespace TaskManagement.Api.Errors;
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Unhandled request exception. TraceId: {TraceId}", context.TraceIdentifier);
        var (status, title, detail) = exception is UnauthorizedAccessException
            ? (StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message)
            : (StatusCodes.Status500InternalServerError, "Internal server error", "An unexpected error occurred.");
        context.Response.StatusCode = status;
        await Results.Problem(statusCode: status, title: title, detail: detail, extensions: new Dictionary<string, object?> { ["traceId"] = context.TraceIdentifier }).ExecuteAsync(context);
        return true;
    }
}
