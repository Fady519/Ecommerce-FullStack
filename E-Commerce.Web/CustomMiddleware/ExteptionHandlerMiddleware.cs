using E_commerce.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Web.CustomMiddleware
{
    public class ExteptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExteptionHandlerMiddleware> _logger;

        public ExteptionHandlerMiddleware(RequestDelegate next , ILogger<ExteptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {

            try
            {
                await _next.Invoke(httpContext);
                await HandleNotFoundEndPointAsync(httpContext);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something Went Wrong");

                
                var Problem = new ProblemDetails()
                {
                    Title = "Error While Processing Http Request",
                    Detail = ex.Message,
                    Instance = httpContext.Request.Path,
                    Status = ex switch
                    {
                        NotFoundException => StatusCodes.Status404NotFound,
                        _=> StatusCodes.Status500InternalServerError
                    }
                };

                httpContext.Response.StatusCode = Problem.Status.Value;
                await httpContext.Response.WriteAsJsonAsync(Problem);

            }

        }


        private static async Task HandleNotFoundEndPointAsync(HttpContext httpContext)
        {
            if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound && !httpContext.Response.HasStarted)
            {
                var Response = new ProblemDetails()
                {
                    Title = "Error While Processing The Http Request - EndPoint Not Found",
                    Detail = $"Endpoints {httpContext.Request.Path} Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Instance = httpContext.Request.Path
                };

                await httpContext.Response.WriteAsJsonAsync(Response);
            }
        }


    }
}
