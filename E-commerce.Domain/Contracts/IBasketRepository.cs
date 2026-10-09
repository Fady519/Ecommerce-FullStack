using E_commerce.Domain.Entities.BasketModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Domain.Contracts
{
    public interface IBasketRepository
    {

        Task<CustomerBasket?> GetBasketAsync(string basketId);
      
        Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket , TimeSpan timeToLive = default);

        Task<bool> DeleteBasketAsync(string basketId);


    }
}
