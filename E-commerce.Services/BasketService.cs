using AutoMapper;
using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entites.BasketModule;
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
        private readonly IMapper _mapper;

        public BasketService(IBasketRepository basketRepository , IMapper mapper) 
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        public async Task<BasketDTO> CreateOrUpdateBasketAsync(BasketDTO basket)
        {
            var CustomerBasket = _mapper.Map<BasketDTO, CustomerBasket>(basket);
            var CreatedOrUpdatedBasket = await _basketRepository.CreateOrUpdateBasketAsync(CustomerBasket);
            return _mapper.Map<CustomerBasket , BasketDTO>(CreatedOrUpdatedBasket);
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

            return _mapper.Map<CustomerBasket , BasketDTO>(Basket!);
        }



    }
}
