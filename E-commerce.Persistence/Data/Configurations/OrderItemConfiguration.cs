using E_commerce.Domain.Entities.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Persistence.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {

            builder.Property(X => X.Price).HasPrecision(8, 2);

            builder.OwnsOne(X => X.Product, OEntity =>
            {
                OEntity.Property(X => X.ProductName).HasMaxLength(100);
                OEntity.Property(X => X.PictureUrl).HasMaxLength(200);
            });

        }
    }
}
