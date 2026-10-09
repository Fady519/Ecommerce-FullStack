using E_commerce.Domain.Entities.BasketModule;

namespace E_commerce.Domain.Contracts
{
    public interface IBasketRepository
    {
        /// <summary>The basket with its items and their products, or null when it doesn't exist.</summary>
        Task<CustomerBasket?> GetBasketAsync(string basketId);

        /// <summary>Saves the basket: creates it, or replaces its items and checkout fields.</summary>
        Task<CustomerBasket> CreateOrUpdateBasketAsync(CustomerBasket basket);

        Task<bool> DeleteBasketAsync(string basketId);
    }
}
