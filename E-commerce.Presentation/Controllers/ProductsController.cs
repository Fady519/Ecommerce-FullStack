using E_commerce.Presentation.Attribute;
using E_commerce.ServicesAbstraction;
using E_commerce.Shared;
using E_commerce.Shared.DTOs.ProductDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Presentation.Controllers
{

    public class ProductsController : ApiBaseController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService) 
        {
            _productService = productService;
        }


        [Authorize]
        [HttpGet]
        [RedisCache]
        public async Task<ActionResult<PaginatedResult<ProductDTOs>>> GetAllProducts([FromQuery] ProductQueryParams queryParams)
        {
            var Products = await _productService.GetAllProductsAsync(queryParams);
            return Ok(Products);

        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTOs>> GetProduct(int id)
        {
            // throw new Exception();
            var Result = await _productService.GetProductByIdAsync(id);
            return HandleResult<ProductDTOs>(Result);

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
