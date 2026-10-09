using E_commerce.Shared.DTOs.IdentityDTOs;
using E_commerce.Shared.CommonResult;

namespace E_commerce.ServicesAbstraction
{
    public interface IAuthenticationService
    {

        Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO);


        Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerDTO);


        Task<bool> CheckEmailAsync(string email);

        /// <summary>The caller's own data; <paramref name="userId"/> must come from the token.</summary>
        Task<Result<CurrentUserDTO>> GetCurrentUserAsync(string userId);

    }
}
