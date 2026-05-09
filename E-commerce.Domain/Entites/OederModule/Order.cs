using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Domain.Entites.OederModule
{
    public class Order : BaseEntitey<Guid>
    {

        public string UserEmail { get; set; } = default!;

        public DateTimeOffset OrderData { get; set; } = DateTimeOffset.Now;

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pendding;

        public OrderAddress Address { get; set; } = default!;

        public DeliveryMethod DeliveryMethod { get; set; } = default!;

        public int DeliveryMethodId { get; set; } // FK

        public ICollection<OrderItem> Items { get; set; } = [];

        public decimal SubTotal { get; set; }

        public decimal GetTotal() => SubTotal + DeliveryMethod.Price;


        public string? PaymentIntentId { get; set; }

    }
}
