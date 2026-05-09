using E_commerce.Domain.Entites.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Persistence.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {

        public void Configure(EntityTypeBuilder<Product> builder)
        {

            builder.Property(X => X.Name)
                .HasMaxLength(100);

            builder.Property(X => X.Description)
                .HasMaxLength(500);

            builder.Property(X => X.PictureUrl)
                .HasMaxLength(200);

            builder.Property(X => X.Price)
                .HasPrecision(18, 2);

            builder.HasOne(X => X.ProductBrand)
                .WithMany()
                .HasForeignKey(X=>X.BrandId);

            builder.HasOne(X => X.ProductType)
                 .WithMany()
                 .HasForeignKey(X => X.TypeId);

        }
            
            



    }
}
