using System.Reflection;
using E_commerce.Domain.Entities.BasketModule;
using E_commerce.Domain.Entities.IdentityModule;
using E_commerce.Domain.Entities.OrderModule;
using E_commerce.Domain.Entities.ProductModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Persistence.Data.DbContexts
{
    /// <summary>
    /// The one and only database of the store: users and roles (ASP.NET Identity), catalog, baskets and orders.
    /// It used to be two databases (store + identity); one database lets an order point at its user with a real
    /// foreign key, and is simpler to back up and to deploy.
    /// </summary>
    public class StoreDbContext(DbContextOptions<StoreDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductBrand> ProductBrands => Set<ProductBrand>();
        public DbSet<ProductType> ProductTypes => Set<ProductType>();
        public DbSet<DeliveryMethod> DeliveryMethods => Set<DeliveryMethod>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<CustomerBasket> Baskets => Set<CustomerBasket>();
        public DbSet<BasketItem> BasketItems => Set<BasketItem>();
        public DbSet<Address> Addresses => Set<Address>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Identity first: it defines the user/role tables our own configurations point at.
            base.OnModelCreating(builder);

            // Short, readable table names instead of "AspNetUsers", "AspNetUserRoles"...
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Money: 18 digits, 2 after the point, for every decimal (the old 8,2 capped totals at 999,999.99).
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }
    }
}
