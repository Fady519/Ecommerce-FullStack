using E_commerce.Shared.DTOs.IdentityDTOs;
using E_Commerce.Web.CommonResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.ServicesAbstraction
{
    public interface IAuthenticationService
    {

        Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO);


        Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerDTO);


        Task<bool> CheckEmailAsync(string email);

        Task<Result<UserDTO>> GetUserByEmailAsync(string email);

    }
}
