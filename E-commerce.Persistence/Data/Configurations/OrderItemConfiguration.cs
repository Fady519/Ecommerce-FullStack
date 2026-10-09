using E_commerce.Domain.Entities.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce.Persistence.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable(t => t.HasCheckConstraint("CK_OrderItems_Quantity", "[Quantity] > 0"));

            // A snapshot of the product at order time (name, picture, and the Price column above).
            // No foreign key on purpose: renaming or removing a product later must not change old orders.
            builder.OwnsOne(X => X.Product, OEntity =>
            {
                OEntity.Property(X => X.ProductId).HasColumnName("ProductId");
                OEntity.Property(X => X.ProductName).HasColumnName("ProductName").HasMaxLength(100);
                OEntity.Property(X => X.PictureUrl).HasColumnName("PictureUrl").HasMaxLength(200);
                OEntity.HasIndex(X => X.ProductId);
            });
        }
    }
}
