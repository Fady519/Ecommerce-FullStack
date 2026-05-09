using E_Commerce.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Web.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class ProductController : ControllerBase
    {

        // Url => api/product/..Params....


        //[HttpGet(template:"{id}")]  // Sent id in  Url 
        //public ActionResult<Product> Get(int id)
        //{
        //    return new Product() { Id = id , Name = "test"};
        //}


        //[HttpGet]
        //public ActionResult<IEnumerable<Product>> GetAll()
        //{
        //    return new List<Product>();
        //}

        //[HttpPost]
        //public ActionResult<Product> AddProduct(Product item)
        //{
        //    return item;
        //}

        //[HttpPut]
        //public ActionResult<Product> UpdateProduct(Product item)
        //{
        //    return item;
        //}

        //[HttpDelete]
        //public ActionResult<Product> DeleteProduct(Product item)
        //{
        //    return item;
        //}



    }
}
