using E_commerce.Domain.Entities.OrderModule;

namespace E_commerce.Domain.Entities.BasketModule
{
    /// <summary>
    /// A shopping basket, stored in SQL Server (it used to live in Redis).
    /// The id is a random string created by the browser, so a guest can fill a basket before logging in.
    /// Prices are NOT stored here: they are always read from the product, so nobody can send a cheaper price.
    /// </summary>
    public class CustomerBasket : BaseEntity<string>
    {
        public ICollection<BasketItem> Items { get; set; } = [];

        /// <summary>Owner when the shopper is logged in; null for a guest basket.</summary>
        public string? UserId { get; set; }

        /// <summary>Chosen at checkout; its price is the shipping cost.</summary>
        public int? DeliveryMethodId { get; set; }
        public DeliveryMethod? DeliveryMethod { get; set; }

        /// <summary>Stripe payment intent created for this basket (see PaymentService).</summary>
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }

        /// <summary>Last change. Lets us clean up baskets that were abandoned long ago.</summary>
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
