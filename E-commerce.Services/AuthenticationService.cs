using E_commerce.Domain.Entities.IdentityModule;
using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.IdentityDTOs;
using E_commerce.Shared.CommonResult;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        /// <summary>
        /// The logged-in user's own data. The id comes from the token ("sub"), never from the URL:
        /// the old version read ?email= from the query string and returned THAT user's data plus a fresh token,
        /// so any logged-in user could take over any account (including the admin's).
        /// </summary>
        public async Task<Result<CurrentUserDTO>> GetCurrentUserAsync(string userId)
        {
            var User = await _userManager.FindByIdAsync(userId);
            if (User is null)
                return Errors.NotFound("User.NotFound", "Your account no longer exists.");

            var Roles = await _userManager.GetRolesAsync(User);
            return new CurrentUserDTO(User.Email!, User.DisplayName, User.PhoneNumber, Roles.ToList(), User.CreatedAt);
        }

        public async Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO)
        {
            // Same message for "no such email" and "wrong password": telling them apart would let
            // anyone find out which emails have an account.
            var User = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (User == null)
            {
                return Errors.InvalidCredentials("User.InvalidCredentials", "Wrong email or password.");
            }

            // Lockout: after 5 wrong passwords the account is blocked for 10 minutes (see AddJwtAuthentication),
            // so a password can't be guessed by trying thousands of them.
            if (await _userManager.IsLockedOutAsync(User))
            {
                return LockedOut();
            }

            var IsPasswordValid = await _userManager.CheckPasswordAsync(User, loginDTO.Password);
            if (!IsPasswordValid)
            {
                // Counts the failure; on the 5th one Identity sets LockoutEnd = now + 10 minutes.
                await _userManager.AccessFailedAsync(User);
                return await _userManager.IsLockedOutAsync(User)
                    ? LockedOut()
                    : Errors.InvalidCredentials("User.InvalidCredentials", "Wrong email or password.");
            }

            // A successful login starts the count again.
            await _userManager.ResetAccessFailedCountAsync(User);
            return await CreateUserResponseAsync(User);
        }

        public async Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerDTO)
        {
            // Checked first so the client gets a clear 409 instead of Identity's generic validation error.
            if (await _userManager.FindByEmailAsync(registerDTO.Email) is not null)
                return Errors.Conflict("User.EmailTaken", "An account with this email already exists.");

            var User = new ApplicationUser()
            {
                Email = registerDTO.Email,
                DisplayName = registerDTO.DisplayName.Trim(),
                PhoneNumber = registerDTO.PhoneNumber,
                // The email is the user name: Identity doesn't allow spaces in user names.
                UserName = registerDTO.Email,
            };
            var IdentityUser = await _userManager.CreateAsync(User , registerDTO.Password);
            if(!IdentityUser.Succeeded)
            {
                // One 400 entry per broken password rule ("PasswordRequiresDigit"...), so the form can show each one.
                return IdentityUser.Errors.Select(E => Errors.Validation(E.Code , E.Description)).ToList();
            }

            // Public sign-up is always a Customer. The role never comes from the request,
            // otherwise anyone could register as Admin.
            var RoleResult = await _userManager.AddToRoleAsync(User, AppRoles.Customer);
            if (!RoleResult.Succeeded)
                throw new InvalidOperationException($"Could not give the Customer role: {string.Join(", ", RoleResult.Errors.Select(E => E.Code))}");

            return await CreateUserResponseAsync(User);
        }


        private static Errors LockedOut() =>
            Errors.InvalidCredentials("User.LockedOut", "Too many failed attempts. The account is locked for 10 minutes.");

        private async Task<UserDTO> CreateUserResponseAsync(ApplicationUser user)
        {
            var Roles = (await _userManager.GetRolesAsync(user)).ToList();
            var ExpiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue("JWTOptions:AccessTokenMinutes", 60));
            return new UserDTO(user.Email!, user.DisplayName, CreateToken(user, Roles, ExpiresAt), Roles, ExpiresAt);
        }

        private string CreateToken(ApplicationUser user, IReadOnlyList<string> roles, DateTime expiresAt)
        {
            var Claims = new List<Claim>()
            {
                // "sub" = the user's id. ASP.NET reads it back as ClaimTypes.NameIdentifier;
                // orders are linked to this id (an email can change, the id never does).
                new Claim(JwtRegisteredClaimNames.Sub , user.Id),
                new Claim(JwtRegisteredClaimNames.Email , user.Email!),
                 new Claim(JwtRegisteredClaimNames.Name , user.DisplayName),
                // Unique token id: useful in logs, and the base for revoking single tokens later.
                new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
            };

            // The role claim is what [Authorize(Policy = "AdminOnly")] checks.
            foreach(var role in roles)
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
                expires: expiresAt,
                claims: Claims,
                signingCredentials: Cred
                );
            return new JwtSecurityTokenHandler().WriteToken(Token);

        }

    }
}
