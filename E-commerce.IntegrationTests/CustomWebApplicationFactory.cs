using E_commerce.Persistence.Data.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace E_commerce.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the app's StoreDbContext registration.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<StoreDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add StoreDbContext using an in-memory database or a real test DB.
                // For a portfolio project, In-Memory or LocalDB is fine. The roadmap asks for "قاعدة بيانات حقيقية" (real DB).
                // Let's use a real LocalDB test instance.
                services.AddDbContext<StoreDbContext>(options =>
                {
                    options.UseSqlServer("Server=.;Database=ECommerceStore_IntegrationTests;Trusted_Connection=True;TrustServerCertificate=True;");
                });
            });
        }
    }
}
