using E_commerce.Domain.Entites.OederModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Services.Specifications
{
    public class OrderSpecifications : BaseSpecifications<Order , Guid>
    {

        public OrderSpecifications(string Email) : base(O => O.UserEmail == Email)
        {
            AddInclude(O => O.DeliveryMethod);
            AddInclude(O => O.Items);
            AddInclude(O => O.OrderData);
        }

        public OrderSpecifications(Guid id, string Email) : base(O => O.ID == id
            && (string.IsNullOrEmpty(Email) || O.UserEmail.ToLower() == Email.ToLower()))
        {
            AddInclude(O => O.DeliveryMethod);
            AddInclude(O => O.Items);
        }

    }
}
