using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entities.OrderModule;
using E_commerce.Domain.Entities.ProductModule;
using E_commerce.Services.Specifications;
using E_commerce.ServicesAbstraction;
using E_commerce.Shared.CommonResult;
using E_commerce.Shared.DTOs.DashboardDTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<DashboardStatisticsDTO>> GetStatisticsAsync()
        {
            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var productRepo = _unitOfWork.GetRepository<Product, int>();

            var orders = await orderRepo.GetAllAsync(new OrderSpecifications(userId: null));
            var allProducts = await productRepo.GetAllAsync();

            var validOrders = orders.Where(o => o.Status != OrderStatus.Cancelled).ToList();

            var totalSales = validOrders.Sum(o => o.SubTotal);
            var totalOrders = validOrders.Count;

            var topSelling = validOrders
                .SelectMany(o => o.Items)
                .GroupBy(i => new { i.Product.ProductId, i.Product.ProductName })
                .Select(g => new TopSellingProductDTO
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantitySold = g.Sum(i => i.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(5)
                .ToList();

            var lowStock = allProducts
                .Where(p => p.Stock <= 10)
                .Select(p => new LowStockProductDTO
                {
                    Id = p.ID,
                    Name = p.Name,
                    Stock = p.Stock
                })
                .OrderBy(p => p.Stock)
                .Take(10)
                .ToList();

            // Last 7 days (UTC), same rule as TotalSales: every order that is not cancelled.
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var salesByDay = validOrders
                .GroupBy(o => DateOnly.FromDateTime(o.OrderDate.UtcDateTime))
                .ToDictionary(g => g.Key, g => g.Sum(o => o.SubTotal));
            var dailySales = Enumerable.Range(0, 7)
                .Select(offset => today.AddDays(offset - 6))
                .Select(day => new DailySalesDTO
                {
                    Date = day,
                    Total = salesByDay.GetValueOrDefault(day)
                })
                .ToList();

            var result = new DashboardStatisticsDTO
            {
                TotalSales = totalSales,
                TotalOrders = totalOrders,
                TopSellingProducts = topSelling,
                LowStockProducts = lowStock,
                DailySales = dailySales
            };

            return Result<DashboardStatisticsDTO>.Ok(result);
        }
    }
}
