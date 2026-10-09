using E_commerce.Domain.Entities.IdentityModule;

namespace E_commerce.Domain.Entities.OrderModule
{
    public class Order : BaseEntity<Guid>
    {
        /// <summary>Who placed the order (foreign key to Users).</summary>
        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        /// <summary>Copy of the email at order time, so the order still shows it if the user changes it later.</summary>
        public string UserEmail { get; set; } = default!;

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        /// <summary>Where to ship. Copied into the order (not linked) so old orders keep their address.</summary>
        public OrderAddress Address { get; set; } = default!;

        public DeliveryMethod DeliveryMethod { get; set; } = default!;

        public int DeliveryMethodId { get; set; } // FK

        public ICollection<OrderItem> Items { get; set; } = [];

        public decimal SubTotal { get; set; }

        public decimal GetTotal() => SubTotal + DeliveryMethod.Price;

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Card;

        /// <summary>Stripe payment intent; only card orders have one.</summary>
        public string? PaymentIntentId { get; set; }

    }
}
