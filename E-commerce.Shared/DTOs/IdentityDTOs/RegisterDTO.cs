using System.ComponentModel.DataAnnotations;

namespace E_commerce.Shared.DTOs.IdentityDTOs
{
    /// <summary>
    /// Sign-up form. There is no UserName field any more: the email IS the user name
    /// (Identity refuses spaces in user names, so "Omar Hassan" used to fail).
    /// There is no Role field either: public sign-up always creates a Customer.
    /// Password rules (8+ chars, upper, lower, digit) are checked by Identity and come back as 400 per rule.
    /// </summary>
    public record RegisterDTO(
        [Required, EmailAddress, MaxLength(256)] string Email,
        [Required, StringLength(100, MinimumLength = 2)] string DisplayName,
        [Required] string Password,
        [Phone, MaxLength(20)] string? PhoneNumber);
}
