using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Shared.DTOs.ProductDTOs
{
    public class ProductDTO
    {

        public int Id {  get; set; }

        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string PictureUrl { get; set; } = default!;

        public decimal Price { get; set; }
        /// <summary>Pieces left (0 = out of stock).</summary>
        public int Stock { get; set; }

        public string ProductType { get; set; } = default!;

        /// <summary>Id of the type, so the admin edit form can preselect it.</summary>
        public int ProductTypeId { get; set; }

        public string ProductBrand { get; set; } = default!;

        /// <summary>Id of the brand, so the admin edit form can preselect it.</summary>
        public int ProductBrandId { get; set; }




    }
}
