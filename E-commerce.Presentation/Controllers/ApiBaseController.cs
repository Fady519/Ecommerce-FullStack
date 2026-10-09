using E_commerce.Shared.CommonResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Presentation.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ApiBaseController : ControllerBase
    {

        /// <summary>
        /// The caller's user id from the token ("sub"), or null for a guest. Even on public endpoints the token
        /// is read when it is sent, so the basket can tell a guest from its owner.
        /// Never take a user id or email from the URL or body: that was the account-takeover bug.
        /// </summary>
        protected string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        /// <summary>Only for [Authorize] actions: a valid token always carries the "sub" claim.</summary>
        protected string RequiredUserId => CurrentUserId
            ?? throw new InvalidOperationException("RequiredUserId used on an action without [Authorize].");

        // Handle Result Without Value
        // if result is success  => return no content
        // if result is failure return problem with status code and error details

        protected IActionResult HandleResult(Result result)
        {
            if(result.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return HandleProblem(result.Errors);
            }
        }


        // Handle Result With Value
        protected ActionResult<TValue> HandleResult<TValue>(Result<TValue> result)
        {
            if(result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return HandleProblem(result.Errors);
            }
        }




        // Helper Method

        private ActionResult HandleProblem(IReadOnlyList<Errors> errors)
        {
            if (errors.Count == 0)
                return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "An unexpected error occurred");

            if(errors.All(e => e.Type == ErrorType.Validation))
                return HandleValidationProblem(errors);

            return HandleSingleErrorProblem(errors[0]);
        }


        private ActionResult HandleSingleErrorProblem(Errors error)
        {
            return Problem
                (
                title: error.Code,
                detail: error.Description,
                type: error.Type.ToString(),
                statusCode: MapErrorTypeToStatusCode(error.Type)
                );
 
        }



        private static int MapErrorTypeToStatusCode(ErrorType errorType) => errorType switch
        {

            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.InvalidCredentials => StatusCodes.Status401Unauthorized,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            _=> StatusCodes.Status500InternalServerError
        };


        private ActionResult HandleValidationProblem(IReadOnlyList<Errors> errors)
        {
            var modelState = new ModelStateDictionary();
            foreach (var error in errors)
                modelState.AddModelError(error.Code, error.Description);
            
            return ValidationProblem(modelState);
        }




    }
}
