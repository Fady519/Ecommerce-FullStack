using E_commerce.Domain.Entities.ProductModule;
using E_commerce.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Services.Specifications
{
    internal static class ProductSpecificationHelper
    {

        public static Expression<Func<Product , bool>> GetProductCriteria(ProductQueryParams queryParams)
        {
            return P => (!queryParams.BrandId.HasValue || P.BrandId == queryParams.BrandId.Value)
            && (!queryParams.TypeId.HasValue || P.TypeId == queryParams.TypeId.Value)
            && (string.IsNullOrEmpty(queryParams.Search) || P.Name.ToLower().Contains(queryParams.Search.ToLower()));
        }

    }
}
