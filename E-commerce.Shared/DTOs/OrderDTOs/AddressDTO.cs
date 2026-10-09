namespace E_commerce.Shared.DTOs.OrderDTOs
{
    // A record (not a class with a primary constructor): records turn the parameters into public
    // properties, so the address can be read from JSON and written back to it.
    public record AddressDTO(string FirstName, string LastName, string City, string Country, string Street);
}