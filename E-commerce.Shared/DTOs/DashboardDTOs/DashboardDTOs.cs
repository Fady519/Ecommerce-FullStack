using System;
using System.Collections.Generic;

namespace E_commerce.Shared.DTOs.DashboardDTOs
{
    public class DashboardStatisticsDTO
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public IEnumerable<LowStockProductDTO> LowStockProducts { get; set; } = new List<LowStockProductDTO>();
        public IEnumerable<TopSellingProductDTO> TopSellingProducts { get; set; } = new List<TopSellingProductDTO>();
        /// <summary>Sales per day for the last 7 days (UTC, oldest first, today last); days without sales are 0.</summary>
        public IEnumerable<DailySalesDTO> DailySales { get; set; } = new List<DailySalesDTO>();
    }

    public class DailySalesDTO
    {
        public DateOnly Date { get; set; }
        public decimal Total { get; set; }
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
