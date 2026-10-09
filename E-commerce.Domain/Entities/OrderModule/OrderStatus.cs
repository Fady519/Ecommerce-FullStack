namespace E_commerce.Domain.Entities.OrderModule
{
    /// <summary>
    /// The life of an order. Stored as text in the database ("Pending"...), so the table is readable without a lookup.
    /// Normal path: Pending → PaymentReceived (card) → Shipped → Delivered.
    /// Cash on delivery skips PaymentReceived: Pending → Shipped → Delivered (paid at the door).
    /// </summary>
    public enum OrderStatus
    {
        Pending,
        PaymentReceived,
        PaymentFailed,
        Shipped,
        Delivered,
        Cancelled
    }
}
