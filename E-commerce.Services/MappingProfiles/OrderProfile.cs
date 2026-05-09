using AutoMapper;
using E_commerce.Domain.Entites.OederModule;
using E_commerce.Shared.DTOs.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Services.MappingProfiles
{
    public class OrderProfile : Profile
    {

        public OrderProfile()
        {

            CreateMap<AddressDTO , OrderAddress>().ReverseMap();

            CreateMap<Order, OrderToReturnDTO>()
                .ForMember(D => D.DeliveryMethod, O => O.MapFrom(src => src.DeliveryMethod.ShortName));

            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(D => D.ProductName , O => O.MapFrom(S => S.Product.ProductName))
                .ForMember(D => D.PictureUrl, O => O.MapFrom<OrderItenPictureUrlResolver>());


            //
            CreateMap<DeliveryMethod, DeliveryMethodDTO>();

        }


    }
}
