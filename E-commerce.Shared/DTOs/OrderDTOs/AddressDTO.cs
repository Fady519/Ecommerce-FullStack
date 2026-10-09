using System.ComponentModel.DataAnnotations;

namespace E_commerce.Shared.DTOs.OrderDTOs
{
    // A record (not a class with a primary constructor): records turn the parameters into public
    // properties, so the address can be read from JSON and written back to it.
    // The limits match the database columns, so a too-long value is a clear 400 instead of a SQL error (500).
    public record AddressDTO(
        [Required, StringLength(50, MinimumLength = 2)] string FirstName,
        [Required, StringLength(50, MinimumLength = 2)] string LastName,
        [Required, StringLength(50, MinimumLength = 2)] string City,
        [Required, StringLength(50, MinimumLength = 2)] string Country,
        [Required, StringLength(100, MinimumLength = 3)] string Street);
}