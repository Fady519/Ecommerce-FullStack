using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entities.BasketModule;
using E_commerce.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Persistence.Repositories
{
    /// <summary>
    /// Baskets in SQL Server (tables Baskets + BasketItems). They used to live in Redis as one JSON string;
    /// now every line is a real row pointing at a real product, so prices always come from the catalog.
    /// </summary>
    public class BasketRepository(StoreDbContext dbContext) : IBasketRepository
    {
        public Task<CustomerBasket?> GetBasketAsync(string basketId)
        {
            // Load everything the basket screen shows: lines → product (name, picture, price, stock)
            // and the chosen delivery method (shipping price).
            return dbContext.Baskets
                .Include(B => B.Items).ThenInclude(I => I.Product)
                .Include(B => B.DeliveryMethod)
                .AsSplitQuery()
                .FirstOrDefaultAsync(B => B.ID == basketId);
        }

        public async Task<CustomerBasket> CreateOrUpdateBasketAsync(CustomerBasket basket)
        {
            var existing = await dbContext.Baskets
                .Include(B => B.Items)
                .FirstOrDefaultAsync(B => B.ID == basket.ID);

            if (existing is null)
            {
                basket.UpdatedAt = DateTimeOffset.UtcNow;
                dbContext.Baskets.Add(basket);
            }
            else
            {
                // The client always sends the whole basket, so we sync the lines:
                // update the quantity of lines that stay, add new ones, remove the missing ones.
                foreach (var oldItem in existing.Items.ToList())
                {
                    var match = basket.Items.FirstOrDefault(I => I.ProductId == oldItem.ProductId);
                    if (match is null) existing.Items.Remove(oldItem);
                    else oldItem.Quantity = match.Quantity;
                }

                foreach (var newItem in basket.Items.Where(I => existing.Items.All(E => E.ProductId != I.ProductId)))
                {
                    existing.Items.Add(new BasketItem { ProductId = newItem.ProductId, Quantity = newItem.Quantity });
                }

                existing.DeliveryMethodId = basket.DeliveryMethodId;
                // Only PaymentService sets these; a normal basket update (which doesn't carry them) must not erase them.
                existing.PaymentIntentId = basket.PaymentIntentId ?? existing.PaymentIntentId;
                existing.ClientSecret = basket.ClientSecret ?? existing.ClientSecret;
                existing.UserId = basket.UserId ?? existing.UserId; // never "forget" the owner
                existing.UpdatedAt = DateTimeOffset.UtcNow;
            }

            await dbContext.SaveChangesAsync();

            // Read it back with products and delivery method loaded (EF fills the navigations of the tracked basket).
            return (await GetBasketAsync(basket.ID))!;
        }

        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            // One SQL DELETE; the items go with it (cascade).
            var deleted = await dbContext.Baskets.Where(B => B.ID == basketId).ExecuteDeleteAsync();
            return deleted > 0;
        }
    }
}
