using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.OrderDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Presentation.Controllers
{
    public class OrderController : ApiBaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<OrderToReturnDTO>> CreateOrder(OrderDTO orderDTO)
        {
            var Result = await _orderService.CreateOrderAsync(orderDTO, GetUserIdFromToken(), GetEmailFromToken());
            return HandleResult(Result);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderToReturnDTO>>> GetOrders()
        {
            var Oredrs = await _orderService.GetAllOrdersAsync(GetUserIdFromToken());
            return HandleResult(Oredrs);
        }

        [Authorize]
        [HttpGet("{Id:guid}")]
        public async Task<ActionResult<OrderToReturnDTO>> GetOrder(Guid Id)
        {
            var Oredr = await _orderService.GetOrderByIdAsync( Id , GetUserIdFromToken());
            return HandleResult(Oredr);
        }


        
        [HttpGet("DeliveryMethods")]
        public async Task<ActionResult<IEnumerable<DeliveryMethodDTO>>> GetDeliverMethods()
        {
            var DeliveryMethods = await _orderService.GetDeliveryMethods();
            return HandleResult(DeliveryMethods);
        }



        // [Authorize] guarantees a valid token, and our tokens always carry the id ("sub") and email claims.
        protected string GetUserIdFromToken() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        protected string GetEmailFromToken()
        {
            return User.FindFirstValue(ClaimTypes.Email)!;
        }

    }
}
