using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Domain.Entities.ProductModule
{
    public class Product : BaseEntity<int>
    {

        

        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string PictureUrl { get; set; } = default!;

        public bool IsActive { get; set; } = true;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;


        public decimal Price { get; set; }

        /// <summary>How many pieces are left. Orders reduce it; it can never go below zero (database check).</summary>
        public int Stock { get; set; }

        
        #region Relationships

        public int BrandId { get; set; }
        public ProductBrand ProductBrand { get; set; } = default!;

        public int TypeId { get; set; }
        public ProductType ProductType { get; set; } = default!;
        #endregion



    }
}
