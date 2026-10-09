using E_commerce.Domain.Entities.BasketModule;
using E_commerce.Shared.DTOs.BasketDTOs;

namespace E_commerce.Services.Mapping
{
    /// <summary>Basket DTO ↔ entity conversions (both directions: the client sends the whole basket).</summary>
    public static class BasketMappings
    {
        public static CustomerBasket ToEntity(this BasketDTO dto) => new()
        {
            id = dto.Id,
            Items = dto.Items.Select(i => new BasketItem
            {
                Id = i.id,
                ProductName = i.ProductName,
                PictureUrl = i.PictureUrl,
                Price = i.Price,
                Quantity = i.Quantity,
            }).ToList(),
            ClientSecret = dto.ClientSecret,
            PaymentIntentId = dto.PaymentIntentId,
            DeliveryMethodId = dto.DeliveryMethodId,
            ShippingPrice = dto.ShippingPrice,
        };

        public static BasketDTO ToDto(this CustomerBasket basket) => new(
            basket.id,
            basket.Items.Select(i => new BasketItemDto(i.Id, i.ProductName, i.PictureUrl, i.Price, i.Quantity)).ToList(),
            basket.ClientSecret,
            basket.PaymentIntentId,
            basket.DeliveryMethodId,
            basket.ShippingPrice);
    }
}
