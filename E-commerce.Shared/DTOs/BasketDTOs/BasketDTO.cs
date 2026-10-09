using System.ComponentModel.DataAnnotations;

namespace E_commerce.Shared.DTOs.BasketDTOs
{
    /// <summary>
    /// The whole basket. The browser creates the <c>Id</c> (a random string) and sends the full basket on every change.
    /// <c>ClientSecret</c>, <c>PaymentIntentId</c> and <c>ShippingPrice</c> are set by the API (input values are ignored).
    /// </summary>
    public record BasketDTO(
        [Required, MaxLength(64)] string Id,
        ICollection<BasketItemDto> Items,
        string? ClientSecret,
        string? PaymentIntentId,
        int? DeliveryMethodId,
        decimal? ShippingPrice);
}
