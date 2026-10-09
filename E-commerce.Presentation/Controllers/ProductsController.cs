using E_commerce.ServicesAbstraction;
using E_commerce.Shared;
using E_commerce.Shared.DTOs.ProductDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace E_commerce.Presentation.Controllers
{
    /// <summary>
    /// The catalog. Open to everyone (no login needed to browse a shop).
    /// Responses are cached in memory for a few minutes ("Catalog" policy, set up in the Web project):
    /// this replaced the Redis cache, so the API no longer needs a Redis server.
    /// </summary>
    [OutputCache(PolicyName = CatalogCachePolicy)]
    public class ProductsController : ApiBaseController
    {
        /// <summary>Name of the output-cache policy (and its tag, used to clear it when the admin edits products).</summary>
        public const string CatalogCachePolicy = "Catalog";

        private readonly IProductService _productService;

        public ProductsController(IProductService productService) 
        {
            _productService = productService;
        }


        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ProductDTO>>> GetAllProducts([FromQuery] ProductQueryParams queryParams)
        {
            var Products = await _productService.GetAllProductsAsync(queryParams);
            return Ok(Products);

        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var Result = await _productService.GetProductByIdAsync(id);
            return HandleResult<ProductDTO>(Result);

        }


        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<TypeDTO>>> GetAllTypes()
        {
            var Types = await _productService.GetAllTypesAsync();
            return Ok(Types);
        }


        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<BrandDTO>>> GetAllBrands()
        {
            var Brands = await _productService.GetAllBrandsAsync();
            return Ok(Brands);
        }

    }
}
