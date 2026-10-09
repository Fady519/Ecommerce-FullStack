using E_commerce.Domain.Entities.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce.Persistence.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(X => X.Name).HasMaxLength(100);
            builder.Property(X => X.Description).HasMaxLength(500);
            builder.Property(X => X.PictureUrl).HasMaxLength(200);

            // The database itself refuses negative prices or stock, even if a bug in the code tries.
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Products_Price", "[Price] >= 0");
                t.HasCheckConstraint("CK_Products_Stock", "[Stock] >= 0");
            });

            // Restrict: a brand/type that still has products can't be deleted by accident.
            builder.HasOne(X => X.ProductBrand)
                .WithMany()
                .HasForeignKey(X => X.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(X => X.ProductType)
                .WithMany()
                .HasForeignKey(X => X.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(X => X.Name);
        }
    }

    public class ProductBrandConfiguration : IEntityTypeConfiguration<ProductBrand>
    {
        public void Configure(EntityTypeBuilder<ProductBrand> builder)
        {
            builder.Property(X => X.Name).HasMaxLength(50);
            builder.HasIndex(X => X.Name).IsUnique();
        }
    }

    public class ProductTypeConfiguration : IEntityTypeConfiguration<ProductType>
    {
        public void Configure(EntityTypeBuilder<ProductType> builder)
        {
            builder.Property(X => X.Name).HasMaxLength(50);
            builder.HasIndex(X => X.Name).IsUnique();
        }
    }
}
