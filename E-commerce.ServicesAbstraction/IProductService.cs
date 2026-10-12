using E_commerce.Shared;
using E_commerce.Shared.DTOs.ProductDTOs;
using E_commerce.Shared.CommonResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.ServicesAbstraction
{
    public interface IProductService
    {


        Task<PaginatedResult<ProductDTO>> GetAllProductsAsync(ProductQueryParams queryParams);

        Task<Result<ProductDTO>> GetProductByIdAsync(int id);

        Task<Result<ProductDTO>> CreateProductAsync(ProductCreateDto productDto);
        Task<Result<ProductDTO>> UpdateProductAsync(int id, ProductUpdateDto productDto);
        Task<Result<bool>> DeleteProductAsync(int id);

        Task<IEnumerable<BrandDTO>> GetAllBrandsAsync();
        Task<Result<BrandDTO>> CreateBrandAsync(string name);
        Task<Result<BrandDTO>> UpdateBrandAsync(int id, string name);
        Task<Result<bool>> DeleteBrandAsync(int id);

        Task<IEnumerable<TypeDTO>> GetAllTypesAsync();
        Task<Result<TypeDTO>> CreateTypeAsync(string name);
        Task<Result<TypeDTO>> UpdateTypeAsync(int id, string name);
        Task<Result<bool>> DeleteTypeAsync(int id);
    }
}
