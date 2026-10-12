using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.ProductDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Threading.Tasks;

namespace E_commerce.Presentation.Controllers.Admin
{
    [Route("api/admin/products")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public class AdminProductsController(IProductService productService, IOutputCacheStore cacheStore) : ApiBaseController
    {
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> Create([FromForm] ProductCreateDto productDto)
        {
            var result = await productService.CreateProductAsync(productDto);
            if (result.IsSuccess)
                await cacheStore.EvictByTagAsync("catalog", default);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDTO>> Update(int id, [FromForm] ProductUpdateDto productDto)
        {
            var result = await productService.UpdateProductAsync(id, productDto);
            if (result.IsSuccess)
                await cacheStore.EvictByTagAsync("catalog", default);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await productService.DeleteProductAsync(id);
            if (result.IsSuccess)
                await cacheStore.EvictByTagAsync("catalog", default);
            return HandleResult(result);
        }
    }
}
