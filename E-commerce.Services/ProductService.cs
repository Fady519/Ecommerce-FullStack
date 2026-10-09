using E_commerce.Services.Mapping;
using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entities.ProductModule;
//using E_commerce.Services.Exception;
using E_commerce.Services.Exceptions;
using E_commerce.Services.Specifications;
using E_commerce.ServicesAbstraction;
using E_commerce.Shared;
using E_commerce.Shared.DTOs.ProductDTOs;
using E_commerce.Shared.CommonResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace E_commerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPictureUrlBuilder _pictures;

        public ProductService(IUnitOfWork unitOfWork , IPictureUrlBuilder pictures) 
        {
            _unitOfWork = unitOfWork;
            _pictures = pictures;
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
            // Count = all matching products (not just this page), so the client can work out the number of pages.
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

    }
}
