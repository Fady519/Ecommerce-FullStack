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

        Task<Result<OrderToReturnDTO>> CreateOrderAsync(OrderDTO orderDTO, string Email);


        Task<Result<IEnumerable<OrderToReturnDTO>>> GetAllOrdersAsync(string Email);


        Task<Result<IEnumerable<DeliveryMethodDTO>>> GetDeliveryMethods();


        Task<Result<OrderToReturnDTO>> GetOrderByIdAsync(Guid id, string Email);

    }
}
