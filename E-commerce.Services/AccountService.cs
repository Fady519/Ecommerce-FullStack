using E_commerce.Domain.Entities.IdentityModule;
using E_commerce.Services.Mapping;
using E_commerce.ServicesAbstraction;
using E_commerce.Shared.CommonResult;
using E_commerce.Shared.DTOs.IdentityDTOs;
using E_commerce.Shared.DTOs.OrderDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Services
{
    /// <summary>Profile, saved address and password of the logged-in user (plus the admin "unlock" action).</summary>
    public class AccountService(UserManager<ApplicationUser> userManager) : IAccountService
    {
        public async Task<Result<CurrentUserDTO>> UpdateProfileAsync(string userId, UpdateProfileDTO dto)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return UserNotFound();

            user.DisplayName = dto.DisplayName.Trim();
            user.PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber) ? null : dto.PhoneNumber.Trim();

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded) return ToValidationErrors(result);

            var roles = await userManager.GetRolesAsync(user);
            return new CurrentUserDTO(user.Email!, user.DisplayName, user.PhoneNumber, roles.ToList(), user.CreatedAt);
        }

        public async Task<Result<AddressDTO>> GetAddressAsync(string userId)
        {
            var user = await LoadWithAddressAsync(userId);
            if (user is null) return UserNotFound();

            // 404 (not an empty object): the checkout page shows an empty form for a user who never saved one.
            if (user.Address is null)
                return Errors.NotFound("Address.NotFound", "You haven't saved an address yet.");

            return user.Address.ToDto();
        }

        public async Task<Result<AddressDTO>> SaveAddressAsync(string userId, AddressDTO dto)
        {
            var user = await LoadWithAddressAsync(userId);
            if (user is null) return UserNotFound();

            // One address per user (1:1 in the database): create it the first time, then just update it.
            user.Address ??= new Address();
            user.Address.CopyFrom(dto);

            // UpdateAsync saves the whole tracked user, including the new/changed Address row.
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded) return ToValidationErrors(result);

            return user.Address.ToDto();
        }

        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordDTO dto)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return Result.Fail(UserNotFound());

            // Identity checks the current password and the rules for the new one
            // (errors like "PasswordMismatch" or "PasswordRequiresDigit" come back as 400).
            var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            return result.Succeeded ? Result.Ok() : Result.Fail(ToValidationErrors(result));
        }

        public async Task<Result> UnlockAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null) return Result.Fail(Errors.NotFound("User.NotFound", $"No account uses {email}."));

            await userManager.SetLockoutEndDateAsync(user, null);
            await userManager.ResetAccessFailedCountAsync(user);
            return Result.Ok();
        }


        private Task<ApplicationUser?> LoadWithAddressAsync(string userId) =>
            userManager.Users.Include(U => U.Address).FirstOrDefaultAsync(U => U.Id == userId);

        private static Errors UserNotFound() => Errors.NotFound("User.NotFound", "Your account no longer exists.");

        private static List<Errors> ToValidationErrors(IdentityResult result) =>
            result.Errors.Select(E => Errors.Validation(E.Code, E.Description)).ToList();
    }
}
