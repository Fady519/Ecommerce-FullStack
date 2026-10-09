using System.ComponentModel.DataAnnotations;

namespace E_commerce.Shared.DTOs.IdentityDTOs
{
    /// <summary>The logged-in user's own data (GET currentUser, GET/PUT account/profile). No token in here on purpose.</summary>
    public record CurrentUserDTO(
        string Email,
        string DisplayName,
        string? PhoneNumber,
        IReadOnlyList<string> Roles,
        DateTimeOffset CreatedAt);

    /// <summary>What a user may change about themselves. Email and roles are not editable here.</summary>
    public record UpdateProfileDTO(
        [Required, StringLength(100, MinimumLength = 2)] string DisplayName,
        [Phone, MaxLength(20)] string? PhoneNumber);

    /// <summary>The current password is required, so a stolen token alone can't lock the owner out.</summary>
    public record ChangePasswordDTO(
        [Required] string CurrentPassword,
        [Required] string NewPassword);

    /// <summary>Admin action: lift a lockout (after 5 wrong passwords) before the 10 minutes are over.</summary>
    public record UnlockUserDTO([Required, EmailAddress] string Email);
}
