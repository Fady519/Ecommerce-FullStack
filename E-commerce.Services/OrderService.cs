using E_commerce.Services.Mapping;
using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entities.OrderModule;
using E_commerce.Domain.Entities.ProductModule;
using E_commerce.Services.Specifications;
using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.OrderDTOs;
using E_commerce.Shared.CommonResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IPictureUrlBuilder _pictures;

        public OrderService(IBasketRepository basketRepository , IUnitOfWork unitOfWork , IPictureUrlBuilder pictures)
        {
            _basketRepository = basketRepository;
            UnitOfWork = unitOfWork;
            _pictures = pictures;
        }

        public IUnitOfWork UnitOfWork { get; }

        public async Task<Result<OrderToReturnDTO>> CreateOrderAsync(OrderDTO orderDTO, string userId, string email)
        {
            
            var OrderAddress = orderDTO.AddressDTO.ToEntity();

            var Basket = await _basketRepository.GetBasketAsync(orderDTO.BasketId);
            if (Basket == null || !Basket.CanBeUsedBy(userId)) return Errors.NotFound("Basket.NotFound", $"the basket with id : {orderDTO.BasketId} is null");
            if (Basket.Items.Count == 0) return Errors.Validation("Basket.Empty", "The basket is empty.");


            // Payment: a card order needs its Stripe payment intent first (POST api/Payment/{basketId}).
            // (Cash on delivery comes in a later phase.) A 400 tells the client what to do; the old code threw a 500.
            if (string.IsNullOrEmpty(Basket.PaymentIntentId))
                return Errors.Validation("Order.PaymentRequired", "Create the payment first: POST api/Payment/{basketId}.");

            var OrderRepo = UnitOfWork.GetRepository<Order, Guid>();

            var Spec = new OrderWithPaymentIntentSpecifications(Basket.PaymentIntentId);
            var ExistOrder = await OrderRepo.GetById(Spec);
            if(ExistOrder is not null)
            {
                OrderRepo.Remove(ExistOrder);
            }


            // The basket was loaded with its products, so names and prices come straight from the catalog.
            List<OrderItem> OrderItems = Basket.Items.Select(item => CreateOrderItem(item, item.Product)).ToList();

            var DeliveryMethod = await UnitOfWork.GetRepository<DeliveryMethod , int>().GetById(orderDTO.DeliveryMethodId);
            if (DeliveryMethod == null) return Errors.NotFound("DeliveryMethod.NotFound", $"the delivery method with id {orderDTO.DeliveryMethodId} is not Found");

            var SubTotal = OrderItems.Sum(I => I.Price * I.Quantity);

            var Order = new Order()
            {
                UserId = userId,
                UserEmail = email,
                Address = OrderAddress,
                DeliveryMethod = DeliveryMethod,
                Items = OrderItems,
                SubTotal = SubTotal,
                PaymentMethod = PaymentMethod.Card,
                PaymentIntentId = Basket.PaymentIntentId
            };

            await OrderRepo.AddAsync(Order);

            int Result =  await UnitOfWork.SaveChangesAsync();
            if (Result == 0) return Errors.Failure("Order.Failure", "Order Cannot Be Created");

            return Order.ToDto(_pictures);

        }


        public async Task<Result<IEnumerable<OrderToReturnDTO>>> GetAllOrdersAsync(string userId)
        {
            
            var Spec = new OrderSpecifications(userId);
            var Orders = await UnitOfWork.GetRepository<Order , Guid>().GetAllAsync(Spec);

            // A new customer simply has no orders yet: that's an empty list (200), not an error (404).
            var Data = Orders.Select(o => o.ToDto(_pictures)).ToList();
            return Result<IEnumerable<OrderToReturnDTO>>.Ok(Data);
            
        }

        public async Task<Result<IEnumerable<DeliveryMethodDTO>>> GetDeliveryMethods()
        {
            var deliveryMethods = await UnitOfWork
                .GetRepository<DeliveryMethod, int>()
                .GetAllAsync();

            if (deliveryMethods == null || !deliveryMethods.Any())
                return Result<IEnumerable<DeliveryMethodDTO>>
                    .Fail(Errors.NotFound("NotFound", "No Delivery Method Found"));

            var data = deliveryMethods.Select(d => d.ToDto()).ToList();

            return Result<IEnumerable<DeliveryMethodDTO>>.Ok(data);
        }

        public async Task<Result<OrderToReturnDTO>> GetOrderByIdAsync(Guid id, string userId)
        {
            var Spec = new OrderSpecifications(id, userId);
            // GetById (one order), not GetAllAsync (a list): a list can never be null, so "not found" was never reported.
            var Order = await UnitOfWork.GetRepository<Order, Guid>().GetById(Spec);

            if (Order is null)
                return Errors.NotFound("Order Not Found");

            
            return Order.ToDto(_pictures);


        }

        // Helper Method
        private static OrderItem CreateOrderItem(Domain.Entities.BasketModule.BasketItem item, Product product)
        {
            return new OrderItem()
            {
                Product = new ProductItemOrdered() { ProductId = product.ID, PictureUrl = product.PictureUrl, ProductName = product.Name },
                Price = product.Price,
                Quantity = item.Quantity


            };
        }


    }
}
