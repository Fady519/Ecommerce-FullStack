using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Web.Factories
{
    public static class ApiResponseFactory
    {

        public static IActionResult GenerateApiValidationError(ActionContext actionContext)
        {
            var Errors = actionContext.ModelState.Where(E => E.Value.Errors.Count > 0)
                .ToDictionary(X => X.Key, X => X.Value.Errors.Select(X => X.ErrorMessage).ToArray());
            var Problem = new ProblemDetails()
            {
                Title = "Validation Error",
                Detail = "one or more validation errors occur",
                Status = StatusCodes.Status400BadRequest,
                Extensions = { { "Errors", Errors } }
            };
            return new BadRequestObjectResult(Problem);
        }

    }
}
