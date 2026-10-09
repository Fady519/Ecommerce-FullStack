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

        /// <summary>
        /// A guest basket (no owner) can be used by anyone who knows its random id.
        /// Once it belongs to a user, only that user can see or change it.
        /// </summary>
        public bool CanBeUsedBy(string? userId) => UserId is null || UserId == userId;

        /// <summary>
        /// Moves the lines of another basket into this one (used when a guest logs in and already had a basket
        /// from another visit). The same product in both: quantities are added, capped at 100 like any line.
        /// </summary>
        public void MergeFrom(CustomerBasket other)
        {
            foreach (var line in other.Items)
            {
                var existing = Items.FirstOrDefault(I => I.ProductId == line.ProductId);
                if (existing is null)
                    Items.Add(new BasketItem { ProductId = line.ProductId, Quantity = Math.Min(100, line.Quantity) });
                else
                    existing.Quantity = Math.Min(100, existing.Quantity + line.Quantity);
            }

            DeliveryMethodId ??= other.DeliveryMethodId;
        }
    }
}
