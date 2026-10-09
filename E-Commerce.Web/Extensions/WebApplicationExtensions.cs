using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entities.IdentityModule;
using E_commerce.Persistence.Data.DataSeed;
using E_commerce.Persistence.Data.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Web.Extensions
{
    public static class WebApplicationExtensions
    {
        /// <summary>
        /// Brings the database up to date (applies pending migrations), then seeds the catalog, the roles
        /// and the admin account. Every step only adds what is missing, so it is safe to run on every start.
        /// </summary>
        public static async Task MigrateAndSeedAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var services = scope.ServiceProvider;

            var db = services.GetRequiredService<StoreDbContext>();
            var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();
            if (pending.Count > 0)
            {
                app.Logger.LogInformation("Applying {Count} migration(s): {Names}", pending.Count, string.Join(", ", pending));
                await db.Database.MigrateAsync();
            }

            await services.GetRequiredKeyedService<IDataInitializer>("Default").InitializeAsync();
            await services.GetRequiredKeyedService<IDataInitializer>("Identity").InitializeAsync();
        }

        /// <summary>
        /// One-off command: dotnet run --project E-Commerce.Web -- --seed-demo
        /// Brings the database up to date, then adds demo customers and ~3 months of orders (see DemoDataSeeding).
        /// </summary>
        public static async Task SeedDemoDataAsync(this WebApplication app)
        {
            await app.MigrateAndSeedAsync();

            await using var scope = app.Services.CreateAsyncScope();
            var services = scope.ServiceProvider;

            var message = await DemoDataSeeding.SeedAsync(
                services.GetRequiredService<StoreDbContext>(),
                services.GetRequiredService<UserManager<ApplicationUser>>(),
                app.Configuration["DemoData:Password"],
                DateTimeOffset.UtcNow);

            app.Logger.LogInformation("Demo data: {Result}", message);
        }
    }
}
