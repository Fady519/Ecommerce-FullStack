using E_commerce.Shared.CommonResult;
using E_commerce.Shared.DTOs.IdentityDTOs;
using E_commerce.Shared.DTOs.OrderDTOs;

namespace E_commerce.ServicesAbstraction
{
    /// <summary>
    /// The logged-in user's own account. Every method takes the user id from the token,
    /// so there is no way to read or change somebody else's data through these endpoints.
    /// </summary>
    public interface IAccountService
    {
        Task<Result<CurrentUserDTO>> UpdateProfileAsync(string userId, UpdateProfileDTO dto);

        Task<Result<AddressDTO>> GetAddressAsync(string userId);

        /// <summary>Creates the saved address the first time, then updates it (one address per user).</summary>
        Task<Result<AddressDTO>> SaveAddressAsync(string userId, AddressDTO dto);

        Task<Result> ChangePasswordAsync(string userId, ChangePasswordDTO dto);

        /// <summary>Admin only: clears a lockout and the failed-attempt counter.</summary>
        Task<Result> UnlockAsync(string email);
    }
}
