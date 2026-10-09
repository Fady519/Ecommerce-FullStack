using E_commerce.Domain.Entities.OrderModule;
using E_commerce.Shared.DTOs.OrderDTOs;

namespace E_commerce.Services.Mapping
{
    /// <summary>Order, address and delivery-method conversions.</summary>
    public static class OrderMappings
    {
        public static OrderAddress ToEntity(this AddressDTO dto) => new()
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            City = dto.City,
            Street = dto.Street,
            Country = dto.Country,
        };

        public static AddressDTO ToDto(this OrderAddress address) =>
            new(address.FirstName, address.LastName, address.City, address.Country, address.Street);

        public static DeliveryMethodDTO ToDto(this DeliveryMethod method) => new()
        {
            Id = method.ID,
            ShortName = method.ShortName,
            Description = method.Description,
            DeliveryTime = method.DeliveryTime,
            Price = method.Price,
        };

        public static OrderItemDTO ToDto(this OrderItem item, IPictureUrlBuilder pictures) =>
            new(item.Product.ProductName, pictures.Build(item.Product.PictureUrl), item.Price, item.Quantity);

        /// <summary>The order must be loaded with its DeliveryMethod and Items (see OrderSpecifications).</summary>
        public static OrderToReturnDTO ToDto(this Order order, IPictureUrlBuilder pictures) => new(
            order.ID,
            order.UserEmail,
            order.Items.Select(i => i.ToDto(pictures)).ToList(),
            order.Address.ToDto(),
            order.DeliveryMethod.ShortName,
            order.Status.ToString(),
            order.OrderDate,
            order.SubTotal,
            order.GetTotal());
    }
}
