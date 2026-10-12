using System.Collections.Generic;

namespace E_commerce.Shared.DTOs.DashboardDTOs
{
    public class DashboardStatisticsDTO
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public IEnumerable<LowStockProductDTO> LowStockProducts { get; set; } = new List<LowStockProductDTO>();
        public IEnumerable<TopSellingProductDTO> TopSellingProducts { get; set; } = new List<TopSellingProductDTO>();
    }

    public class LowStockProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Stock { get; set; }
    }

    public class TopSellingProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
    }
}
