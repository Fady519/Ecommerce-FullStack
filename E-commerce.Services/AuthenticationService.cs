using E_commerce.Domain.Entities.IdentityModule;
using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.IdentityDTOs;
using E_commerce.Shared.CommonResult;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(UserManager<ApplicationUser> userManager , IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<bool> CheckEmailAsync(string email)
        {
            var User = await _userManager.FindByEmailAsync(email);
            return User != null ;
        }

        public async Task<Result<UserDTO>> GetUserByEmailAsync(string email)
        {
            var User = await _userManager.FindByEmailAsync(email);
            if(User is null)
            {
                return Errors.NotFound("User.NotFound" , $"No User With Email {email} Was Found");
            }

            return new UserDTO(User.Email!, User.DisplayName , await CreateTokenAsync(User));
        }

        public async Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO)
        {
            
            var User = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (User == null)
            {
                return Errors.InvalidCredentials("User.InvalidCredentials");
            }

            var IsPasswordValid = await _userManager.CheckPasswordAsync(User, loginDTO.Password);
            if (!IsPasswordValid)
            {
                return Errors.InvalidCredentials("User.InvalidCredentials");
            }

            var Token = await CreateTokenAsync(User);
            return new UserDTO(User.Email!, User.DisplayName, Token);
        }

        public async Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerDTO)
        {

            var User = new ApplicationUser()
            {
                Email = registerDTO.Email,
                DisplayName = registerDTO.DisplayName,
                PhoneNumber = registerDTO.PhoneNumber,
                UserName = registerDTO.UserName,
            };
            var IdentityUser = await _userManager.CreateAsync(User , registerDTO.Password);
            if(IdentityUser.Succeeded)
            {
                var Token = await CreateTokenAsync(User);
                return new UserDTO(User.Email, User.DisplayName, Token);
            }

            return IdentityUser.Errors.Select(E => Errors.Validation(E.Code , E.Description)).ToList();
            
        }


        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var Claims = new List<Claim>()
            {
                // "sub" = the user's id. ASP.NET reads it back as ClaimTypes.NameIdentifier;
                // orders are linked to this id (an email can change, the id never does).
                new Claim(JwtRegisteredClaimNames.Sub , user.Id),
                new Claim(JwtRegisteredClaimNames.Email , user.Email!),
                 new Claim(JwtRegisteredClaimNames.Name , user.UserName!),
            };

            var Roles = await _userManager.GetRolesAsync(user);
            foreach(var role in Roles)
            {
                Claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Checked at startup (AddJwtAuthentication), so it is never missing here.
            var SecretKey = _configuration["JWTOptions:SecretKey"]
                ?? throw new InvalidOperationException("JWTOptions:SecretKey is not configured.");
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var Cred = new SigningCredentials(Key , SecurityAlgorithms.HmacSha256);

            var Token = new JwtSecurityToken(
                issuer: _configuration["JWTOptions:Issuer"],
                audience: _configuration["JWTOptions:Audience"],
                expires: DateTime.UtcNow.AddHours(1),
                claims: Claims,
                signingCredentials: Cred
                );
            return new JwtSecurityTokenHandler().WriteToken(Token);

        }

    }
}
