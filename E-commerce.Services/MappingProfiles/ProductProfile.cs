using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using E_commerce.Domain.Entities.ProductModule;
using E_commerce.Shared.DTOs.ProductDTOs;



namespace E_commerce.Services.MappingProfiles
{
    public class ProductProfile : Profile
    {

        public ProductProfile() 
        {

            CreateMap<ProductBrand, BrandDTO>();
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.ProductType.Name))
                .ForMember(dest => dest.ProductBrand, opt => opt.MapFrom(src => src.ProductBrand.Name))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom<ProductPictureUrlResolver>());
                


            CreateMap<ProductType , TypeDTO>();
            CreateMap<ProductBrand, BrandDTO>();
        }


    }
}
