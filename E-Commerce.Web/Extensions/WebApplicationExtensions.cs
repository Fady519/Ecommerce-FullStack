using E_commerce.Domain.Contracts;
using E_commerce.Persistence.Data.DbContexts;
using E_commerce.Persistence.IdentityData;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace E_Commerce.Web.Extensions
{
    public static class WebApplicationExtensions
    {

        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {

            await using var scope = app.Services.CreateAsyncScope();

            var dbContextService = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
            var pendingMigration = await dbContextService.Database.GetPendingMigrationsAsync();
            if (pendingMigration.Any())
                await dbContextService.Database.MigrateAsync();


            return app;
        }

        public static async Task<WebApplication> MigrateIdentityDatabaseAsync(this WebApplication app)
        {

            await using var scope = app.Services.CreateAsyncScope();

            var dbContextService = scope.ServiceProvider.GetRequiredService<StoreIdentityDbContext>();
            var pendingMigration = await dbContextService.Database.GetPendingMigrationsAsync();
            if (pendingMigration.Any())
                await dbContextService.Database.MigrateAsync();


            return app;
        }

        public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
        {
             await using var scope = app.Services.CreateAsyncScope();

            var dataInitializer = scope.ServiceProvider.GetRequiredKeyedService<IDataInitializer>("Default");
            await dataInitializer.InitializeAsync();

            return app;
        }

        public static async Task<WebApplication> SeedIdentityDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();

            var dataInitializer = scope.ServiceProvider.GetRequiredKeyedService<IDataInitializer>("Identity");
            await dataInitializer.InitializeAsync();

            return app;
        }

    }
}
