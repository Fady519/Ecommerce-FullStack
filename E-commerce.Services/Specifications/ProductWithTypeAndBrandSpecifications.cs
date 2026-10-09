using E_commerce.Domain.Entities.ProductModule;
using E_commerce.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Services.Specifications
{
    internal class ProductWithTypeAndBrandSpecifications : BaseSpecifications<Product , int>
    {

        public ProductWithTypeAndBrandSpecifications(int id) : base(P=>P.ID == id)
        {
            AddInclude(P => P.ProductType);
            AddInclude(P => P.ProductBrand);
        }

        public ProductWithTypeAndBrandSpecifications(ProductQueryParams queryParams) 
            : base(ProductSpecificationHelper.GetProductCriteria(queryParams))
        {
        
            AddInclude(P=>P.ProductType);
            AddInclude(P=>P.ProductBrand);

            switch(queryParams.Sort)
            {
                case ProductSortingOptions.NameAsc:
                    AddOrderBy(P => P.Name);
                    break;

                case ProductSortingOptions.NameDesc:
                    AddOrderByDescending(P => P.Name);
                    break;

                case ProductSortingOptions.PriceAsc:
                    AddOrderBy(P => P.Price);
                    break;

                case ProductSortingOptions.PriceDesc:
                    AddOrderByDescending(P => P.Price);
                    break;

                default:
                    AddOrderBy(P => P.ID);
                    break;
            }

            ApplyPagination(queryParams.PageSize, queryParams.PageIndex);


        }



    }
}
