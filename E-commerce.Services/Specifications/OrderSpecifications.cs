using E_commerce.Domain.Entities.OrderModule;

namespace E_commerce.Services.Specifications
{
    public class OrderSpecifications : BaseSpecifications<Order , Guid>
    {
        /// <summary>All orders of one user, newest first.</summary>
        public OrderSpecifications(string userId) : base(O => O.UserId == userId)
        {
            AddInclude(O => O.DeliveryMethod);
            AddInclude(O => O.Items);
            AddOrderByDescending(O => O.OrderDate);
        }

        /// <summary>
        /// One order, only if it belongs to this user (by id, not by email: an email can change, the id can't).
        /// </summary>
        public OrderSpecifications(Guid id, string userId) : base(O => O.ID == id && O.UserId == userId)
        {
            AddInclude(O => O.DeliveryMethod);
            AddInclude(O => O.Items);
        }
    }
}
