using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entities.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace E_commerce.Persistence.Data.DataSeed
{
    /// <summary>The store owner's account, read from the "AdminUser" config section.</summary>
    public class AdminUserOptions
    {
        public const string SectionName = "AdminUser";

        public string Email { get; set; } = "";
        public string DisplayName { get; set; } = "Store Admin";

        /// <summary>A secret: user-secrets locally, environment variable (AdminUser__Password) on the server.</summary>
        public string? Password { get; set; }
    }

    /// <summary>
    /// Makes sure the roles exist (Admin, Customer) and that the store owner has an Admin account.
    /// Safe to run on every start: it only creates what is missing.
    /// </summary>
    public class IdentityDataInitializer(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<AdminUserOptions> adminOptions,
        ILogger<IdentityDataInitializer> logger) : IDataInitializer
    {
        public async Task InitializeAsync()
        {
            foreach (var role in AppRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var admin = adminOptions.Value;
            if (string.IsNullOrWhiteSpace(admin.Email))
            {
                logger.LogWarning("No admin account seeded: AdminUser:Email is not configured.");
                return;
            }

            var user = await userManager.FindByEmailAsync(admin.Email);
            if (user is null)
            {
                // The password is never written in code or appsettings.json (the old seed had "P@ssw@rd" in git).
                if (string.IsNullOrWhiteSpace(admin.Password))
                {
                    logger.LogWarning("No admin account seeded: set AdminUser:Password (user-secrets locally, env var on the server).");
                    return;
                }

                user = new ApplicationUser
                {
                    // UserName = email: Identity does not allow spaces in user names ("Fady Kaiser" failed silently).
                    UserName = admin.Email,
                    Email = admin.Email,
                    EmailConfirmed = true,
                    DisplayName = admin.DisplayName,
                };

                var created = await userManager.CreateAsync(user, admin.Password);
                if (!created.Succeeded)
                    throw new InvalidOperationException("Could not create the admin account: "
                        + string.Join(" ", created.Errors.Select(E => E.Description)));

                logger.LogInformation("Admin account created: {Email}", admin.Email);
            }

            if (!await userManager.IsInRoleAsync(user, AppRoles.Admin))
                await userManager.AddToRoleAsync(user, AppRoles.Admin);
        }
    }
}
