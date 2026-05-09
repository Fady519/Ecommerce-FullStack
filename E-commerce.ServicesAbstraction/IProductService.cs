using E_commerce.Shared;
using E_commerce.Shared.DTOs.ProductDTOs;
using E_Commerce.Web.CommonResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.ServicesAbstraction
{
    public interface IProductService
    {


        Task<PaginatedResult<ProductDTOs>> GetAllProductsAsync(ProductQueryParams queryParams);


        Task<Result<ProductDTOs>> GetProductByIdAsync(int id);

        Task<IEnumerable<BrandDTO>> GetAllBrandsAsync();


        Task<IEnumerable<TypeDTO>> GetAllTypesAsync();



    }
}
