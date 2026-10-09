using E_commerce.Domain.Entities.BasketModule;
using E_commerce.Domain.Entities.IdentityModule;
using E_commerce.Domain.Entities.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce.Persistence.Data.Configurations
{
    public class DeliveryMethodConfiguration : IEntityTypeConfiguration<DeliveryMethod>
    {
        public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
        {
            builder.Property(X => X.ShortName).HasMaxLength(50);
            builder.Property(X => X.DeliveryTime).HasMaxLength(50);
            builder.Property(X => X.Description).HasMaxLength(100);
            builder.ToTable(t => t.HasCheckConstraint("CK_DeliveryMethods_Price", "[Price] >= 0"));
        }
    }

    public class CustomerBasketConfiguration : IEntityTypeConfiguration<CustomerBasket>
    {
        public void Configure(EntityTypeBuilder<CustomerBasket> builder)
        {
            builder.ToTable("Baskets");
            builder.Property(X => X.ID).HasMaxLength(64);
            builder.Property(X => X.PaymentIntentId).HasMaxLength(100);
            builder.Property(X => X.ClientSecret).HasMaxLength(200);

            // If a delivery method is removed, baskets simply lose their choice.
            builder.HasOne(X => X.DeliveryMethod)
                .WithMany()
                .HasForeignKey(X => X.DeliveryMethodId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(X => X.Items)
                .WithOne(X => X.Basket)
                .HasForeignKey(X => X.BasketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(X => X.UpdatedAt);

            // Optional owner. No navigation property (the domain doesn't need one), just a real foreign key.
            // Deleting a user deletes their basket too.
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(X => X.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
    {
        public void Configure(EntityTypeBuilder<BasketItem> builder)
        {
            builder.ToTable(t => t.HasCheckConstraint("CK_BasketItems_Quantity", "[Quantity] BETWEEN 1 AND 100"));

            // Removing a product from the catalog also removes it from open baskets.
            builder.HasOne(X => X.Product)
                .WithMany()
                .HasForeignKey(X => X.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // A product appears once per basket; more pieces = a bigger Quantity.
            builder.HasIndex(X => new { X.BasketId, X.ProductId }).IsUnique();
        }
    }

    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");
            builder.Property(X => X.FirstName).HasMaxLength(50);
            builder.Property(X => X.LastName).HasMaxLength(50);
            builder.Property(X => X.City).HasMaxLength(50);
            builder.Property(X => X.Country).HasMaxLength(50);
            builder.Property(X => X.Street).HasMaxLength(100);

            // One saved address per user, deleted together with the user.
            builder.HasOne(X => X.User)
                .WithOne(X => X.Address)
                .HasForeignKey<Address>(X => X.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
