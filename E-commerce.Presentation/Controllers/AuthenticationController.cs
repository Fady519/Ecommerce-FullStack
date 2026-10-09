using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.IdentityDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace E_commerce.Presentation.Controllers
{
    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }


        /// <summary>Returns a JWT. 5 wrong passwords lock the account for 10 minutes; max 10 tries per minute per IP.</summary>
        [HttpPost("Login")]
        [EnableRateLimiting(AuthPolicies.AuthRateLimit)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var Result = await _authenticationService.LoginAsync(loginDTO);
            return HandleResult(Result);
        }


        /// <summary>Creates a Customer account and logs it in. 409 if the email is taken, 400 per broken password rule.</summary>
        [HttpPost("Register")]
        [EnableRateLimiting(AuthPolicies.AuthRateLimit)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            var Result = await _authenticationService.RegisterAsync(registerDTO);
            return HandleResult(Result);
        }


        /// <summary>For the sign-up form ("this email is already registered"). Rate limited, since it reveals whether an account exists.</summary>
        [HttpGet("emailExists")]
        [EnableRateLimiting(AuthPolicies.AuthRateLimit)]
        public async Task<ActionResult<bool>> CheckEmail(string email)
        {
            var result = await _authenticationService.CheckEmailAsync(email);
            return Ok(result);
        }

        /// <summary>
        /// The logged-in user, read from the token. The old version took ?email= from the URL and returned that
        /// user's data AND a new token for them: any logged-in user could become any other user.
        /// </summary>
        [Authorize]
        [HttpGet("CurrentUser")]
        public async Task<ActionResult<CurrentUserDTO>> GetCurrentUser()
        {
            var Result = await _authenticationService.GetCurrentUserAsync(RequiredUserId);
            return HandleResult(Result);
        }

    }
}
