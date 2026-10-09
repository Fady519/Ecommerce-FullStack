using E_commerce.Services.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Web.Infrastructure
{
    /// <summary>
    /// Last line of defence: turns an unhandled exception into an RFC 9457 Problem Details response.
    /// - Known "not found" exceptions → 404 with their message (safe to show).
    /// - Anything else → 500 with a generic message. The real error is only written to the log,
    ///   because exception messages can leak table names, SQL or file paths to attackers.
    /// </summary>
    public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetails, ILogger<GlobalExceptionHandler> logger)
        : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, title, detail) = exception switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Not found", exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "Server error", "Something went wrong on our side. Please try again."),
            };

            if (status >= 500)
                logger.LogError(exception, "Unhandled exception for {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

            httpContext.Response.StatusCode = status;
            return await problemDetails.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails { Status = status, Title = title, Detail = detail },
            });
        }
    }
}
