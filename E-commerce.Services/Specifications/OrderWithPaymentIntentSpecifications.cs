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

        /// <param name="includeItems">Load the order lines too (needed to give their stock back when the order is replaced).</param>
        public OrderWithPaymentIntentSpecifications(string IntentId, bool includeItems = false) : base(O => O.PaymentIntentId == IntentId)
        {
            if (includeItems)
                AddInclude(O => O.Items);
        }

    }
}
