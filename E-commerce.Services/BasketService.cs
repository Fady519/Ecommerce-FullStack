using E_commerce.Services.Mapping;
using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entities.BasketModule;
using E_commerce.Services.Exceptions;
using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.BasketDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Services
{

    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;

        public BasketService(IBasketRepository basketRepository) 
        {
            _basketRepository = basketRepository;
        }

        public async Task<BasketDTO> CreateOrUpdateBasketAsync(BasketDTO basket)
        {
            var CreatedOrUpdatedBasket = await _basketRepository.CreateOrUpdateBasketAsync(basket.ToEntity())
                ?? throw new BasketNotFoundException(basket.Id);
            return CreatedOrUpdatedBasket.ToDto();
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            return await _basketRepository.DeleteBasketAsync(id);
        }

        public async Task<BasketDTO> GetBasketAsync(string id)
        {
            var Basket = await _basketRepository.GetBasketAsync(id);

            if(Basket == null) 
                throw new BasketNotFoundException(id);

            return Basket.ToDto();
        }



    }
}
