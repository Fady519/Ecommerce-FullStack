using AutoMapper;
using E_commerce.Domain.Entites.ProductModule;
using E_commerce.Shared.DTOs.ProductDTOs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Services.MappingProfiles
{
    public class ProductPictureUrlResolver : IValueResolver<Product, ProductDTOs, string>
    {
        private readonly IConfiguration _configuration;

        public ProductPictureUrlResolver(IConfiguration configuration) 
        {
            _configuration = configuration;
        }


        public string Resolve(Product source, ProductDTOs destination, string destMember, ResolutionContext context)
        {
            
            if(string.IsNullOrEmpty(source.PictureUrl))
                return string.Empty;

            if(source.PictureUrl.StartsWith("http"))
                return source.PictureUrl;

            var BaseUrl = _configuration.GetSection("URLs")["BaseUrl"];
            if(string.IsNullOrEmpty(BaseUrl)) return string.Empty;

            var PicUrl = $"{BaseUrl}{source.PictureUrl}";
            return PicUrl;



        }
    }
}
