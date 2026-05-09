using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Domain.Entites.OederModule
{
    public class OrderItem : BaseEntitey<int>
    {

        public ProductItemOrdered Product { get; set; } = default!;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

    }
}
