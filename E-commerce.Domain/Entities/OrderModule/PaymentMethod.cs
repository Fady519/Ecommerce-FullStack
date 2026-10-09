namespace E_commerce.Domain.Entities.OrderModule
{
    /// <summary>How the customer pays. Stored as text ("Card", "CashOnDelivery").</summary>
    public enum PaymentMethod
    {
        /// <summary>Paid online through Stripe (test mode).</summary>
        Card,

        /// <summary>Paid in cash to the courier; no Stripe payment intent is needed.</summary>
        CashOnDelivery
    }
}
