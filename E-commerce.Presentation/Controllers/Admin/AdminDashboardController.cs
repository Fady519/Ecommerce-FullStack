using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.DashboardDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace E_commerce.Presentation.Controllers.Admin
{
    [Route("api/admin/dashboard")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public class AdminDashboardController(IDashboardService dashboardService) : ApiBaseController
    {
        [HttpGet]
        public async Task<ActionResult<DashboardStatisticsDTO>> GetStatistics()
        {
            var result = await dashboardService.GetStatisticsAsync();
            return HandleResult(result);
        }
    }
}
