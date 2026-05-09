using AutoMapper;
using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entites.OederModule;
using E_commerce.Domain.Entites.ProductModule;
using E_commerce.Services.Specifications;
using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.OrderDTOs;
using E_Commerce.Web.CommonResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IBasketRepository _basketRepository;

        public OrderService(IMapper mapper , IBasketRepository basketRepository , IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _basketRepository = basketRepository;
            UnitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork { get; }

        public async Task<Result<OrderToReturnDTO>> CreateOrderAsync(OrderDTO orderDTO, string Email)
        {
            
            var OrderAddress = _mapper.Map<OrderAddress>(orderDTO.AddressDTO);

            var Basket = await _basketRepository.GetBasketAsync(orderDTO.BasketId);
            if (Basket == null) return Errors.NotFound("Basket.NotFound", $"the basket with id : {orderDTO.BasketId} is null");


            // Payment
            ArgumentNullException.ThrowIfNullOrEmpty(Basket.PaymentIntentId);

            var OrderRepo = UnitOfWork.GetRepository<Order, Guid>();

            var Spec = new OrderWithPaymentIntentSpecifications(Basket.PaymentIntentId);
            var ExistOrder = await OrderRepo.GetById(Spec);
            if(ExistOrder is not null)
            {
                OrderRepo.Remove(ExistOrder);
            }


            List<OrderItem> OrderItems = new List<OrderItem>();
            foreach (var item in Basket.Items)
            {
                var product = await UnitOfWork.GetRepository<Product, int>().GetById(item.Id);
                if (product == null) return Errors.NotFound("Product.NotFound", $"the product with id {item.Id} is null");
                CreateOrderItem(item, product);

                OrderItems.Add(CreateOrderItem(item, product));
            }

            var DeliveryMethod = await UnitOfWork.GetRepository<DeliveryMethod , int>().GetById(orderDTO.DeliveryMethodId);
            if (DeliveryMethod == null) return Errors.NotFound("DeliveryMethod.NotFound", $"the product with id {orderDTO.DeliveryMethodId} is not Found");

            var SubTotal = OrderItems.Sum(I => I.Price * I.Quantity);

            var Order = new Order()
            {
                Address = OrderAddress,
                DeliveryMethod = DeliveryMethod,
                Items = OrderItems,
                SubTotal = SubTotal,
                UserEmail = Email,
                PaymentIntentId = Basket.PaymentIntentId
            };

            await UnitOfWork.GetRepository<Order , Guid>().AddAsync(Order);

            int Result =  await UnitOfWork.SaveChangesAsync();
            if (Result == 0) return Errors.Failure("Order.Failure", "Order Cannot Be Created");

            return _mapper.Map<OrderToReturnDTO>(Order);

        }


        public async Task<Result<IEnumerable<OrderToReturnDTO>>> GetAllOrdersAsync(string Email)
        {
            
            var Spec = new OrderSpecifications(Email);
            var Orders = await UnitOfWork.GetRepository<Order , Guid>().GetAllAsync(Spec);

            if (!Orders.Any())
                return Errors.NotFound("Order Not Found");

            var Data = _mapper.Map<IEnumerable<OrderToReturnDTO>>(Orders);
            return Result<IEnumerable<OrderToReturnDTO>>.Ok(Data);
            
        }

        //public Task<Result<IEnumerable<DeliveryMethodDTO>>> GetDeliveryMethods()
        //{
        //    var DeliveryMethods = UnitOfWork.GetRepository<DeliveryMethod , int>().GetAllAsync();
        //    if (!DeliveryMethods.Any())
        //        return Errors.NotFound("NotFound", "No Delivery Method Found");

        //    var Data = _mapper.Map<IEnumerable<DeliveryMethod>, IEnumerable<DeliveryMethodDTO>>(DeliveryMethods);
        //    return Result<IEnumerable<DeliveryMethodDTO>>.Ok(Data);

        //}

        public async Task<Result<IEnumerable<DeliveryMethodDTO>>> GetDeliveryMethods()
        {
            var deliveryMethods = await UnitOfWork
                .GetRepository<DeliveryMethod, int>()
                .GetAllAsync();

            if (deliveryMethods == null || !deliveryMethods.Any())
                return Result<IEnumerable<DeliveryMethodDTO>>
                    .Fail(Errors.NotFound("NotFound", "No Delivery Method Found"));

            var data = _mapper.Map<IEnumerable<DeliveryMethodDTO>>(deliveryMethods);

            return Result<IEnumerable<DeliveryMethodDTO>>.Ok(data);
        }

        public async Task<Result<OrderToReturnDTO>> GetOrderByIdAsync(Guid id, string Email)
        {
            var Spec = new OrderSpecifications(id, Email);
            var Order = await UnitOfWork.GetRepository<Order, Guid>().GetAllAsync(Spec);

            if (Order is null)
                return Errors.NotFound("Order Not Found");

            
            return _mapper.Map<OrderToReturnDTO>(Order);


        }

        // Helper Method
        private static OrderItem CreateOrderItem(Domain.Entites.BasketModule.BasketItems item, Product product)
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
