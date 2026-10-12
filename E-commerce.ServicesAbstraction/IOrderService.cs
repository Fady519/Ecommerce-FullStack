using E_commerce.Shared.DTOs.OrderDTOs;
using E_commerce.Shared.CommonResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.ServicesAbstraction
{
    public interface IOrderService
    {

        /// <param name="userId">From the token ("sub" claim) — never from the request body.</param>
        /// <param name="email">Saved on the order as a snapshot.</param>
        Task<Result<OrderToReturnDTO>> CreateOrderAsync(OrderDTO orderDTO, string userId, string email);


        Task<Result<IEnumerable<OrderToReturnDTO>>> GetAllOrdersAsync(string userId);


        Task<Result<IEnumerable<DeliveryMethodDTO>>> GetDeliveryMethods();

        Task<Result<OrderToReturnDTO>> GetOrderByIdAsync(Guid id, string userId);

        Task<Result<IEnumerable<OrderToReturnDTO>>> GetAllOrdersForAdminAsync();

        Task<Result<OrderToReturnDTO>> UpdateOrderStatusAsync(Guid orderId, string statusText);
    }
}
