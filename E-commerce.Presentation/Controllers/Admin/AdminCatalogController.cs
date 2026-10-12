using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.ProductDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Threading.Tasks;

namespace E_commerce.Presentation.Controllers.Admin
{
    [Route("api/admin/catalog")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public class AdminCatalogController(IProductService productService, IOutputCacheStore cacheStore) : ApiBaseController
    {
        [HttpPost("brands")]
        public async Task<ActionResult<BrandDTO>> CreateBrand([FromForm] string name)
        {
            var result = await productService.CreateBrandAsync(name);
            if (result.IsSuccess)
                await cacheStore.EvictByTagAsync("catalog", default);
            return HandleResult(result);
        }

        [HttpPut("brands/{id}")]
        public async Task<ActionResult<BrandDTO>> UpdateBrand(int id, [FromForm] string name)
        {
            var result = await productService.UpdateBrandAsync(id, name);
            if (result.IsSuccess)
                await cacheStore.EvictByTagAsync("catalog", default);
            return HandleResult(result);
        }

        [HttpDelete("brands/{id}")]
        public async Task<ActionResult<bool>> DeleteBrand(int id)
        {
            var result = await productService.DeleteBrandAsync(id);
            if (result.IsSuccess)
                await cacheStore.EvictByTagAsync("catalog", default);
            return HandleResult(result);
        }

        [HttpPost("types")]
        public async Task<ActionResult<TypeDTO>> CreateType([FromForm] string name)
        {
            var result = await productService.CreateTypeAsync(name);
            if (result.IsSuccess)
                await cacheStore.EvictByTagAsync("catalog", default);
            return HandleResult(result);
        }

        [HttpPut("types/{id}")]
        public async Task<ActionResult<TypeDTO>> UpdateType(int id, [FromForm] string name)
        {
            var result = await productService.UpdateTypeAsync(id, name);
            if (result.IsSuccess)
                await cacheStore.EvictByTagAsync("catalog", default);
            return HandleResult(result);
        }

        [HttpDelete("types/{id}")]
        public async Task<ActionResult<bool>> DeleteType(int id)
        {
            var result = await productService.DeleteTypeAsync(id);
            if (result.IsSuccess)
                await cacheStore.EvictByTagAsync("catalog", default);
            return HandleResult(result);
        }
    }
}
