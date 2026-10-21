using System.Net;
using System.Net.Http;
using System.Text.Json;
using E_commerce.Domain.Entities.BasketModule;
using E_commerce.Domain.Entities.IdentityModule;
using E_commerce.Domain.Entities.OrderModule;
using E_commerce.Domain.Entities.ProductModule;
using E_commerce.Persistence.Data.DbContexts;
using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.OrderDTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace E_commerce.IntegrationTests
{
    /// <summary>
    /// Admin product edit (brand/type ids), safe image deletion, card-order retry stock,
    /// the dashboard's 7-day sales series and the CORS allow-list.
    /// </summary>
    public class CatalogOrderAndDashboardTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public CatalogOrderAndDashboardTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetProducts_ReturnsBrandAndTypeIds()
        {
            var response = await _client.GetAsync("/api/products?pageSize=5");
            response.EnsureSuccessStatusCode();

            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var products = json.RootElement.GetProperty("data").EnumerateArray().ToList();

            Assert.NotEmpty(products);
            Assert.All(products, p =>
            {
                Assert.True(p.GetProperty("productBrandId").GetInt32() > 0);
                Assert.True(p.GetProperty("productTypeId").GetInt32() > 0);
            });
        }

        [Fact]
        public async Task DeleteProduct_KeepsSharedAndSeedImages_DeletesUnusedUpload()
        {
            await using var scope = _factory.Services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
            var products = scope.ServiceProvider.GetRequiredService<IProductService>();
            var webRoot = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>().WebRootPath;

            // An upload (the "{guid}_{name}" shape ImageService writes) and a seed-style picture.
            var uploadUrl = $"products/{Guid.NewGuid()}_shared-test.png";
            var seedUrl = $"images/products/zz-test-seed-{Guid.NewGuid():N}.jpeg";
            var uploadPath = Path.Combine(webRoot, uploadUrl.Replace('/', Path.DirectorySeparatorChar));
            var seedPath = Path.Combine(webRoot, seedUrl.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(uploadPath)!);
            Directory.CreateDirectory(Path.GetDirectoryName(seedPath)!);
            await File.WriteAllTextAsync(uploadPath, "test");
            await File.WriteAllTextAsync(seedPath, "test");

            try
            {
                var first = NewProduct(db, uploadUrl);
                var second = NewProduct(db, uploadUrl);
                var seeded = NewProduct(db, seedUrl);
                db.Products.AddRange(first, second, seeded);
                await db.SaveChangesAsync();

                Assert.True((await products.DeleteProductAsync(first.ID)).IsSuccess);
                Assert.True(File.Exists(uploadPath), "A picture still used by another product must stay.");

                Assert.True((await products.DeleteProductAsync(second.ID)).IsSuccess);
                Assert.False(File.Exists(uploadPath), "An upload nobody uses any more should be removed.");

                Assert.True((await products.DeleteProductAsync(seeded.ID)).IsSuccess);
                Assert.True(File.Exists(seedPath), "Seed pictures are never deleted.");
            }
            finally
            {
                File.Delete(uploadPath);
                File.Delete(seedPath);
            }
        }

        [Fact]
        public async Task CardOrderRetry_RestoresStockOfReplacedOrder_AndShowsInTodaysSales()
        {
            var email = $"retry-{Guid.NewGuid():N}@test.local";
            var basketId = $"test-basket-{Guid.NewGuid():N}";
            const int quantity = 2;
            int productId;
            int stockBefore;
            string userId;
            decimal salesTodayBefore;

            // Arrange: a customer with a card basket that already has a payment intent.
            await using (var scope = _factory.Services.CreateAsyncScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
                var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var user = new ApplicationUser { UserName = email, Email = email, DisplayName = "Retry Test" };
                Assert.True((await users.CreateAsync(user, "Test@12345")).Succeeded);
                userId = user.Id;

                var product = await db.Products.OrderBy(P => P.ID).FirstAsync(P => P.Stock >= 10);
                productId = product.ID;
                stockBefore = product.Stock;

                db.Baskets.Add(new CustomerBasket
                {
                    ID = basketId,
                    UserId = userId,
                    PaymentIntentId = $"pi_test_{Guid.NewGuid():N}",
                    Items = [new BasketItem { ProductId = productId, Quantity = quantity }],
                });
                await db.SaveChangesAsync();

                salesTodayBefore = await SalesTodayAsync(scope.ServiceProvider);
            }

            try
            {
                // Act: the same card checkout twice (e.g. the customer retried); each call is its own request.
                decimal subTotal = 0;
                for (var attempt = 0; attempt < 2; attempt++)
                {
                    await using var scope = _factory.Services.CreateAsyncScope();
                    var db = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
                    var orders = scope.ServiceProvider.GetRequiredService<IOrderService>();
                    var deliveryId = await db.DeliveryMethods.Select(D => D.ID).FirstAsync();

                    var result = await orders.CreateOrderAsync(
                        new OrderDTO(basketId, deliveryId, new AddressDTO("Test", "User", "Cairo", "Egypt", "1 Test St")),
                        userId, email);
                    Assert.True(result.IsSuccess);
                    subTotal = result.Value.SubTotal;
                }

                // Assert: one order left, and the stock went down once, not twice.
                await using (var scope = _factory.Services.CreateAsyncScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
                    Assert.Equal(1, await db.Orders.CountAsync(O => O.UserId == userId));
                    var stockAfter = await db.Products.Where(P => P.ID == productId).Select(P => P.Stock).SingleAsync();
                    Assert.Equal(stockBefore - quantity, stockAfter);

                    // The new (pending) order counts in today's bar of the dashboard's 7-day series.
                    Assert.Equal(salesTodayBefore + subTotal, await SalesTodayAsync(scope.ServiceProvider));
                }
            }
            finally
            {
                await using var scope = _factory.Services.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
                await db.Orders.Where(O => O.UserId == userId).ExecuteDeleteAsync();
                await db.Baskets.Where(B => B.ID == basketId).ExecuteDeleteAsync();
                await db.Products.Where(P => P.ID == productId).ExecuteUpdateAsync(S => S.SetProperty(P => P.Stock, stockBefore));
                var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await users.FindByIdAsync(userId);
                if (user is not null) await users.DeleteAsync(user);
            }
        }

        [Fact]
        public async Task Dashboard_ReturnsLast7DaysSeries_EndingToday()
        {
            await using var scope = _factory.Services.CreateAsyncScope();
            var stats = (await scope.ServiceProvider.GetRequiredService<IDashboardService>().GetStatisticsAsync()).Value;

            var days = stats.DailySales.ToList();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            Assert.Equal(7, days.Count);
            Assert.Equal(today, days[^1].Date);
            for (var i = 0; i < days.Count; i++)
                Assert.Equal(today.AddDays(i - 6), days[i].Date);
            Assert.All(days, d => Assert.True(d.Total >= 0));
            Assert.True(days.Sum(d => d.Total) <= stats.TotalSales);
        }

        [Theory]
        [InlineData("http://localhost:5173", true)]   // listed in appsettings.Development.json
        [InlineData("https://evil.example", false)]   // never "any origin" any more
        public async Task Cors_AllowsOnlyConfiguredOrigins(string origin, bool allowed)
        {
            var request = new HttpRequestMessage(HttpMethod.Options, "/api/products");
            request.Headers.Add("Origin", origin);
            request.Headers.Add("Access-Control-Request-Method", "GET");

            var response = await _client.SendAsync(request);

            var hasHeader = response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values);
            Assert.Equal(allowed, hasHeader && values!.Contains(origin));
        }

        private static async Task<decimal> SalesTodayAsync(IServiceProvider services)
        {
            var stats = (await services.GetRequiredService<IDashboardService>().GetStatisticsAsync()).Value;
            return stats.DailySales.Single(D => D.Date == DateOnly.FromDateTime(DateTime.UtcNow)).Total;
        }

        private static Product NewProduct(StoreDbContext db, string pictureUrl) => new()
        {
            Name = "Image test product",
            Description = "Created by an integration test.",
            PictureUrl = pictureUrl,
            Price = 10,
            Stock = 1,
            BrandId = db.ProductBrands.Select(B => B.ID).First(),
            TypeId = db.ProductTypes.Select(T => T.ID).First(),
        };
    }
}
