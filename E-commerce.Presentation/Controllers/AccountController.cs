using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.IdentityDTOs;
using E_commerce.Shared.DTOs.OrderDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Presentation.Controllers
{
    /// <summary>
    /// "My account" for any logged-in user (customer or admin). There is no {id} in any route:
    /// the user is always the one in the token, so nobody can reach another person's data from here.
    /// </summary>
    [Authorize]
    public class AccountController(IAccountService accountService, IAuthenticationService authenticationService) : ApiBaseController
    {
        [HttpGet("profile")]
        public async Task<ActionResult<CurrentUserDTO>> GetProfile() =>
            HandleResult(await authenticationService.GetCurrentUserAsync(RequiredUserId));

        [HttpPut("profile")]
        public async Task<ActionResult<CurrentUserDTO>> UpdateProfile(UpdateProfileDTO dto) =>
            HandleResult(await accountService.UpdateProfileAsync(RequiredUserId, dto));

        /// <summary>The saved shipping address (404 if none yet). The checkout form is pre-filled with it.</summary>
        [HttpGet("address")]
        public async Task<ActionResult<AddressDTO>> GetAddress() =>
            HandleResult(await accountService.GetAddressAsync(RequiredUserId));

        [HttpPut("address")]
        public async Task<ActionResult<AddressDTO>> SaveAddress(AddressDTO dto) =>
            HandleResult(await accountService.SaveAddressAsync(RequiredUserId, dto));

        /// <summary>204 on success. Needs the current password.</summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto) =>
            HandleResult(await accountService.ChangePasswordAsync(RequiredUserId, dto));
    }
}
