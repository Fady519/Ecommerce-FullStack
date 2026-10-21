using E_commerce.Domain.Entities.ProductModule;
using E_commerce.Shared.DTOs.ProductDTOs;

namespace E_commerce.Services.Mapping
{
    /// <summary>
    /// Entity → DTO conversions for the product catalog.
    /// Written by hand instead of AutoMapper: every field is visible here, and a renamed
    /// property becomes a compile error instead of a silent null at runtime.
    /// </summary>
    public static class ProductMappings
    {
        public static BrandDTO ToDto(this ProductBrand brand) => new() { Id = brand.ID, Name = brand.Name };

        public static TypeDTO ToDto(this ProductType type) => new() { Id = type.ID, Name = type.Name };

        public static ProductDTO ToDto(this Product product, IPictureUrlBuilder pictures) => new()
        {
            Id = product.ID,
            Name = product.Name,
            Description = product.Description,
            PictureUrl = pictures.Build(product.PictureUrl),
            Price = product.Price,
            Stock = product.Stock,
            ProductBrand = product.ProductBrand?.Name ?? string.Empty,
            ProductBrandId = product.BrandId,
            ProductType = product.ProductType?.Name ?? string.Empty,
            ProductTypeId = product.TypeId,
        };
    }
}
