using E_commerce.Domain.Entities.IdentityModule;
using E_commerce.Shared.DTOs.OrderDTOs;

namespace E_commerce.Services.Mapping
{
    /// <summary>The user's saved address ⇄ AddressDTO (the same DTO the checkout form sends with an order).</summary>
    public static class AccountMappings
    {
        public static AddressDTO ToDto(this Address address) =>
            new(address.FirstName, address.LastName, address.City, address.Country, address.Street);

        public static void CopyFrom(this Address address, AddressDTO dto)
        {
            address.FirstName = dto.FirstName.Trim();
            address.LastName = dto.LastName.Trim();
            address.City = dto.City.Trim();
            address.Country = dto.Country.Trim();
            address.Street = dto.Street.Trim();
        }
    }
}
