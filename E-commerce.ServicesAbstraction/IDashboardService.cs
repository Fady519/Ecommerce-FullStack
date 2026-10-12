using E_commerce.Shared.CommonResult;
using E_commerce.Shared.DTOs.DashboardDTOs;
using System.Threading.Tasks;

namespace E_commerce.ServicesAbstraction
{
    public interface IDashboardService
    {
        Task<Result<DashboardStatisticsDTO>> GetStatisticsAsync();
    }
}
