using E_commerce.Domain.Entities.BasketModule;
using E_commerce.Shared.DTOs.BasketDTOs;

namespace E_commerce.Services.Mapping
{
    /// <summary>Basket DTO ⇄ entity conversions (both directions: the client sends the whole basket).</summary>
    public static class BasketMappings
    {
        /// <summary>
        /// Keeps only what the client is allowed to decide: which products, how many, and the delivery method.
        /// The same product sent twice becomes one line (quantities added, max 100).
        /// </summary>
        public static CustomerBasket ToEntity(this BasketDTO dto) => new()
        {
            ID = dto.Id,
            Items = dto.Items
                .GroupBy(I => I.Id)
                .Select(G => new BasketItem { ProductId = G.Key, Quantity = Math.Min(100, G.Sum(I => I.Quantity)) })
                .ToList(),
            DeliveryMethodId = dto.DeliveryMethodId,
        };

        /// <summary>The basket must be loaded with Items → Product and DeliveryMethod (see BasketRepository).</summary>
        public static BasketDTO ToDto(this CustomerBasket basket, IPictureUrlBuilder pictures) => new(
            basket.ID,
            basket.Items
                .Select(I => new BasketItemDto(I.ProductId, I.Quantity, I.Product.Name, pictures.Build(I.Product.PictureUrl), I.Product.Price))
                .ToList(),
            basket.ClientSecret,
            basket.PaymentIntentId,
            basket.DeliveryMethodId,
            basket.DeliveryMethod?.Price);
    }
}
