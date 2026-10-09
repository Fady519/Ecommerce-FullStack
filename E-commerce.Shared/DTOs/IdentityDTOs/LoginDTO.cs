using System.ComponentModel.DataAnnotations;

namespace E_commerce.Shared.DTOs.IdentityDTOs
{
    public record LoginDTO(
        [Required, EmailAddress] string Email,
        [Required] string Password);
}
