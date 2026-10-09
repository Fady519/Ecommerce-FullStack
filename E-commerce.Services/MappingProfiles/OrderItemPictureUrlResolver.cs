using AutoMapper;
using AutoMapper.Execution;
using E_commerce.Domain.Entities.OrderModule;
using E_commerce.Shared.DTOs.OrderDTOs;
using Microsoft.Extensions.Configuration;

namespace E_commerce.Services.MappingProfiles
{
    public class OrderItemPictureUrlResolver : IValueResolver<OrderItem, OrderItemDTO, string>
    {
        private readonly IConfiguration _configuration;

        public OrderItemPictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(OrderItem source, OrderItemDTO destination, string destMember, ResolutionContext context)
        {
            if(string.IsNullOrWhiteSpace(source.Product.PictureUrl)) return string.Empty;

            if(source.Product.PictureUrl.StartsWith("http")) return source.Product.PictureUrl;

            var BaseUrl = _configuration.GetSection("URLs")["ApiUrl"];
            if(string.IsNullOrEmpty(BaseUrl)) return string.Empty;

            return $"{BaseUrl} {source.Product.PictureUrl}";
        }


    }

}