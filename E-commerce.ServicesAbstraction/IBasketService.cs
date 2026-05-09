using E_commerce.Shared.DTOs.BasketDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.ServicesAbstraction
{
    public interface IBasketService
    {


        Task<BasketDTO> GetBasketAsync(string id);

        Task<BasketDTO> CreateOrUpdateBasketAsync(BasketDTO basket);


        Task<bool> DeleteBasketAsync(string id);

    }
}
