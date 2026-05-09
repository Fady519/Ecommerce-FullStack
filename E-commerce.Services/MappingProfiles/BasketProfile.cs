using AutoMapper;
using E_commerce.Domain.Entites.BasketModule;
using E_commerce.Shared.DTOs.BasketDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Services.MappingProfiles
{
    public class BasketProfile : Profile
    {

        public BasketProfile() 
        {

            CreateMap<CustomerBasket, BasketDTO>().ReverseMap();

            CreateMap<BasketItems, BasketItemDto>().ReverseMap();

        }

    }
}
