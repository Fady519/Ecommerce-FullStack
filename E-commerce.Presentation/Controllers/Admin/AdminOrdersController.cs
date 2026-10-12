using E_commerce.ServicesAbstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace E_commerce.Presentation.Controllers.Admin
{
    [Route("api/admin/orders")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public class AdminOrdersController(IOrderService orderService) : ApiBaseController
    {
        [HttpGet]
        public async Task<ActionResult<System.Collections.Generic.IEnumerable<E_commerce.Shared.DTOs.OrderDTOs.OrderToReturnDTO>>> GetAllOrders()
        {
            var result = await orderService.GetAllOrdersForAdminAsync();
            return HandleResult(result);
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<E_commerce.Shared.DTOs.OrderDTOs.OrderToReturnDTO>> UpdateOrderStatus(Guid id, [FromBody] string status)
        {
            var result = await orderService.UpdateOrderStatusAsync(id, status);
            return HandleResult(result);
        }
    }
}
