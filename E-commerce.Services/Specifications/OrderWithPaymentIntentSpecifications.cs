using E_commerce.Domain.Entities.OrderModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Services.Specifications
{
    public class OrderWithPaymentIntentSpecifications : BaseSpecifications<Order , Guid>
    {

        public OrderWithPaymentIntentSpecifications(string IntentId) : base(O => O.PaymentIntentId == IntentId)
        {


        }

    }
}
