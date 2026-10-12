using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace E_commerce.Shared.DTOs.ProductDTOs
{
    public class ProductUpdateDto
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Description { get; set; } = default!;

        [Range(0.01, 100000)]
        public decimal Price { get; set; }

        [Range(0, 100000)]
        public int Stock { get; set; }

        private int _brandId;
        public int BrandId
        {
            get => _brandId;
            set => _brandId = value != 0 ? value : _brandId;
        }

        public int ProductBrandId
        {
            get => _brandId;
            set => _brandId = value != 0 ? value : _brandId;
        }

        private int _typeId;
        public int TypeId
        {
            get => _typeId;
            set => _typeId = value != 0 ? value : _typeId;
        }

        public int ProductTypeId
        {
            get => _typeId;
            set => _typeId = value != 0 ? value : _typeId;
        }

        public bool IsActive { get; set; } = true;

        public string? PictureUrl { get; set; }

        public IFormFile? Image { get; set; }
    }
}
