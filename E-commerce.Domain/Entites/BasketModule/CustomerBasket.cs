using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Domain.Entites.BasketModule
{
    public class CustomerBasket
    {

        public string id { get; set; } = default!;


        public ICollection<BasketItems> Items { get; set; } = [];


        public string? ClientSecret { get; set; }

        public string? PaymentIntentId { get; set; }


        public int? DeliveryMethodId { get; set; }

        public decimal? ShippingPrice { get; set; }

    }
}
