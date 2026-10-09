using E_commerce.Domain.Entities.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce.Persistence.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(X => X.UserEmail).HasMaxLength(256);
            builder.Property(X => X.PaymentIntentId).HasMaxLength(100);

            // Saved as text ("Pending") instead of a number (0), so the table reads clearly.
            builder.Property(X => X.Status).HasConversion<string>().HasMaxLength(30);
            builder.Property(X => X.PaymentMethod).HasConversion<string>().HasMaxLength(30);

            // The shipping address is copied into the order's own columns (Address_City...).
            builder.OwnsOne(X => X.Address, OEntity =>
            {
                OEntity.Property(X => X.FirstName).HasMaxLength(50);
                OEntity.Property(X => X.LastName).HasMaxLength(50);
                OEntity.Property(X => X.City).HasMaxLength(50);
                OEntity.Property(X => X.Country).HasMaxLength(50);
                OEntity.Property(X => X.Street).HasMaxLength(100);
            });

            // Restrict: deleting a user or a delivery method must not silently delete order history.
            builder.HasOne(X => X.User)
                .WithMany()
                .HasForeignKey(X => X.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(X => X.DeliveryMethod)
                .WithMany()
                .HasForeignKey(X => X.DeliveryMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            // Deleting an order deletes its lines (they mean nothing alone).
            builder.HasMany(X => X.Items)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(X => new { X.UserId, X.OrderDate });
            builder.HasIndex(X => X.PaymentIntentId);
        }
    }
}
