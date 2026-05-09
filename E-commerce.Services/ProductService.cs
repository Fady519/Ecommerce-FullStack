using AutoMapper;
using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entites.ProductModule;
//using E_commerce.Services.Exception;
using E_commerce.Services.Exceptions;
using E_commerce.Services.Specifications;
using E_commerce.ServicesAbstraction;
using E_commerce.Shared;
using E_commerce.Shared.DTOs.ProductDTOs;
using E_Commerce.Web.CommonResult;
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
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork , IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<IEnumerable<BrandDTO>> GetAllBrandsAsync()
        {
            var Brands = await _unitOfWork.GetRepository<ProductBrand, int>().GetAllAsync();
            return _mapper.Map<IEnumerable<BrandDTO>>(Brands);
        }

        public async Task<PaginatedResult<ProductDTOs>> GetAllProductsAsync(ProductQueryParams queryParams)
        {
            var Spec = new ProductWithTypeAndBrandSpecifications(queryParams);

            var Products = await _unitOfWork.GetRepository<Product, int>().GetAllAsync(Spec);
            var DataToReturn = _mapper.Map<IEnumerable<ProductDTOs>>(Products);
            var CountOfReturnedData = DataToReturn.Count();
            var CountSpec = new ProductCountSpecification(queryParams);
            var CountOfAllProducts = await _unitOfWork.GetRepository<Product, int>().CountAsync(CountSpec);
            return new PaginatedResult<ProductDTOs>(queryParams.PageIndex, CountOfReturnedData, 0, DataToReturn);
        }

        public async Task<IEnumerable<TypeDTO>> GetAllTypesAsync()
        {
            var types = await _unitOfWork.GetRepository<ProductType, int>().GetAllAsync();
            return _mapper.Map<IEnumerable<TypeDTO>>(types);
        }

        public async Task<Result<ProductDTOs>> GetProductByIdAsync(int id)
        {
            var Spec = new ProductWithTypeAndBrandSpecifications(id);

            var Product = await _unitOfWork.GetRepository<Product, int>().GetById(Spec);

            if (Product == null)
                return Errors.NotFound("Product.NotFound" , $"Product With id {id} is Not Found");
            

            return _mapper.Map<ProductDTOs>(Product);
        }

    }
}
