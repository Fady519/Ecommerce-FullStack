using E_commerce.Domain.Entities.ProductModule;

namespace E_commerce.Domain.Entities.BasketModule
{
    /// <summary>One line of a basket: which product, and how many.</summary>
    public class BasketItem : BaseEntity<int>
    {
        public string BasketId { get; set; } = default!;
        public CustomerBasket Basket { get; set; } = default!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public int Quantity { get; set; }
    }
}
