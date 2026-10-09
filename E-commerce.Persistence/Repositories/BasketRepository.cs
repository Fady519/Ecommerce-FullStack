using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entities.BasketModule;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_commerce.Persistence.Repositories
{

    

    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;
        public BasketRepository(IConnectionMultiplexer connection) 
        {
            _database = connection.GetDatabase();
        }

        public async Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket, TimeSpan timeToLive = default)
        {
            var jsonBasket = JsonSerializer.Serialize(basket);
            var IsCreatedOrUpdate = await _database.StringSetAsync(basket.id, jsonBasket , 
                (timeToLive == default) ? TimeSpan.FromDays(7) : timeToLive);

            if(IsCreatedOrUpdate)
            {
                //var Basket = await _database.StringGetAsync(basket.id);
                //return JsonSerializer.Deserialize<CustomerBasket>(Basket);
                return await GetBasketAsync(basket.id);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteBasketAsync(string basketId)
        {
           return await _database.KeyDeleteAsync(basketId);
        }

        public async Task<CustomerBasket?> GetBasketAsync(string basketId)
        {
            var Basket = await _database.StringGetAsync(basketId);
            if(Basket.IsNullOrEmpty)
            {
                return null;
            }
            else
            {
                return JsonSerializer.Deserialize<CustomerBasket>(Basket.ToString());
            }
        }
    }
}
