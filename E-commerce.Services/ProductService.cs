using E_commerce.Services.Mapping;
using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entities.ProductModule;
using E_commerce.Services.Exceptions;
using E_commerce.Services.Specifications;
using E_commerce.ServicesAbstraction;
using E_commerce.Shared;
using E_commerce.Shared.DTOs.ProductDTOs;
using E_commerce.Shared.CommonResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPictureUrlBuilder _pictures;
        private readonly IImageService _imageService;

        public ProductService(IUnitOfWork unitOfWork, IPictureUrlBuilder pictures, IImageService imageService) 
        {
            _unitOfWork = unitOfWork;
            _pictures = pictures;
            _imageService = imageService;
        }

        public async Task<IEnumerable<BrandDTO>> GetAllBrandsAsync()
        {
            var Brands = await _unitOfWork.GetRepository<ProductBrand, int>().GetAllAsync();
            return Brands.Select(b => b.ToDto()).ToList();
        }

        public async Task<PaginatedResult<ProductDTO>> GetAllProductsAsync(ProductQueryParams queryParams)
        {
            var Spec = new ProductWithTypeAndBrandSpecifications(queryParams);
            var Products = await _unitOfWork.GetRepository<Product, int>().GetAllAsync(Spec);
            var DataToReturn = Products.Select(p => p.ToDto(_pictures)).ToList();
            var CountSpec = new ProductCountSpecification(queryParams);
            var CountOfAllProducts = await _unitOfWork.GetRepository<Product, int>().CountAsync(CountSpec);
            return new PaginatedResult<ProductDTO>(queryParams.PageIndex, queryParams.PageSize, CountOfAllProducts, DataToReturn);
        }

        public async Task<IEnumerable<TypeDTO>> GetAllTypesAsync()
        {
            var types = await _unitOfWork.GetRepository<ProductType, int>().GetAllAsync();
            return types.Select(t => t.ToDto()).ToList();
        }

        public async Task<Result<ProductDTO>> GetProductByIdAsync(int id)
        {
            var Spec = new ProductWithTypeAndBrandSpecifications(id);
            var Product = await _unitOfWork.GetRepository<Product, int>().GetById(Spec);
            if (Product == null)
                return Errors.NotFound("Product.NotFound" , $"Product With id {id} is Not Found");
            return Product.ToDto(_pictures);
        }

        public async Task<Result<ProductDTO>> CreateProductAsync(ProductCreateDto productDto)
        {
            if (productDto.BrandId <= 0 || await _unitOfWork.GetRepository<ProductBrand, int>().GetById(productDto.BrandId) is null)
                return Errors.Validation("Product.InvalidBrand", "The selected Brand does not exist. Please select a valid brand.");

            if (productDto.TypeId <= 0 || await _unitOfWork.GetRepository<ProductType, int>().GetById(productDto.TypeId) is null)
                return Errors.Validation("Product.InvalidType", "The selected Category / Type does not exist. Please select a valid category.");

            string pictureUrl = !string.IsNullOrWhiteSpace(productDto.PictureUrl) ? productDto.PictureUrl : "images/products/placeholder.png";
            if (productDto.Image != null)
                pictureUrl = await _imageService.SaveImageAsync(productDto.Image, "products");

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                BrandId = productDto.BrandId,
                TypeId = productDto.TypeId,
                IsActive = productDto.IsActive,
                PictureUrl = pictureUrl
            };

            await _unitOfWork.GetRepository<Product, int>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return await GetProductByIdAsync(product.ID);
        }

        public async Task<Result<ProductDTO>> UpdateProductAsync(int id, ProductUpdateDto productDto)
        {
            var product = await _unitOfWork.GetRepository<Product, int>().GetById(id);
            if (product == null)
                return Errors.NotFound("Product.NotFound", $"Product With id {id} is Not Found");

            if (productDto.BrandId <= 0 || await _unitOfWork.GetRepository<ProductBrand, int>().GetById(productDto.BrandId) is null)
                return Errors.Validation("Product.InvalidBrand", "The selected Brand does not exist. Please select a valid brand.");

            if (productDto.TypeId <= 0 || await _unitOfWork.GetRepository<ProductType, int>().GetById(productDto.TypeId) is null)
                return Errors.Validation("Product.InvalidType", "The selected Category / Type does not exist. Please select a valid category.");

            if (productDto.Image != null)
            {
                if (!string.IsNullOrEmpty(product.PictureUrl) && !product.PictureUrl.StartsWith("http"))
                    _imageService.DeleteImage(product.PictureUrl);
                product.PictureUrl = await _imageService.SaveImageAsync(productDto.Image, "products");
            }
            else if (!string.IsNullOrWhiteSpace(productDto.PictureUrl))
            {
                product.PictureUrl = productDto.PictureUrl;
            }

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.Stock = productDto.Stock;
            product.BrandId = productDto.BrandId;
            product.TypeId = productDto.TypeId;
            product.IsActive = productDto.IsActive;

            _unitOfWork.GetRepository<Product, int>().Update(product);
            await _unitOfWork.SaveChangesAsync();
            return await GetProductByIdAsync(product.ID);
        }

        public async Task<Result<bool>> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.GetRepository<Product, int>().GetById(id);
            if (product == null)
                return Errors.NotFound("Product.NotFound", $"Product With id {id} is Not Found");

            if (!string.IsNullOrEmpty(product.PictureUrl))
                _imageService.DeleteImage(product.PictureUrl);

            _unitOfWork.GetRepository<Product, int>().Remove(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<Result<BrandDTO>> CreateBrandAsync(string name)
        {
            var brand = new ProductBrand { Name = name };
            await _unitOfWork.GetRepository<ProductBrand, int>().AddAsync(brand);
            await _unitOfWork.SaveChangesAsync();
            return brand.ToDto();
        }

        public async Task<Result<BrandDTO>> UpdateBrandAsync(int id, string name)
        {
            var brand = await _unitOfWork.GetRepository<ProductBrand, int>().GetById(id);
            if (brand == null)
                return Errors.NotFound("Brand.NotFound", $"Brand With id {id} is Not Found");

            brand.Name = name;
            _unitOfWork.GetRepository<ProductBrand, int>().Update(brand);
            await _unitOfWork.SaveChangesAsync();
            return brand.ToDto();
        }

        public async Task<Result<bool>> DeleteBrandAsync(int id)
        {
            var brand = await _unitOfWork.GetRepository<ProductBrand, int>().GetById(id);
            if (brand == null)
                return Errors.NotFound("Brand.NotFound", $"Brand With id {id} is Not Found");

            _unitOfWork.GetRepository<ProductBrand, int>().Remove(brand);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<Result<TypeDTO>> CreateTypeAsync(string name)
        {
            var type = new ProductType { Name = name };
            await _unitOfWork.GetRepository<ProductType, int>().AddAsync(type);
            await _unitOfWork.SaveChangesAsync();
            return type.ToDto();
        }

        public async Task<Result<TypeDTO>> UpdateTypeAsync(int id, string name)
        {
            var type = await _unitOfWork.GetRepository<ProductType, int>().GetById(id);
            if (type == null)
                return Errors.NotFound("Type.NotFound", $"Type With id {id} is Not Found");

            type.Name = name;
            _unitOfWork.GetRepository<ProductType, int>().Update(type);
            await _unitOfWork.SaveChangesAsync();
            return type.ToDto();
        }

        public async Task<Result<bool>> DeleteTypeAsync(int id)
        {
            var type = await _unitOfWork.GetRepository<ProductType, int>().GetById(id);
            if (type == null)
                return Errors.NotFound("Type.NotFound", $"Type With id {id} is Not Found");

            _unitOfWork.GetRepository<ProductType, int>().Remove(type);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
