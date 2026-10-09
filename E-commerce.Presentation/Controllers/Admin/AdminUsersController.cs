using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.IdentityDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Presentation.Controllers.Admin
{
    /// <summary>
    /// Admin-only user actions. Every controller under /api/admin uses the AdminOnly policy:
    /// no token → 401, a customer's token → 403.
    /// </summary>
    [Route("api/admin/users")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public class AdminUsersController(IAccountService accountService) : ApiBaseController
    {
        /// <summary>Lifts a lockout (5 wrong passwords) before the 10 minutes are over. 204 on success.</summary>
        [HttpPost("unlock")]
        public async Task<IActionResult> Unlock(UnlockUserDTO dto) =>
            HandleResult(await accountService.UnlockAsync(dto.Email));
    }
}
