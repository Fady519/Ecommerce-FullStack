using E_commerce.Domain.Entities.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Persistence.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(X => X.SubTotal)
                .HasPrecision(8, 2);

            builder.OwnsOne(X => X.Address, OEntity =>
            {
                OEntity.Property(X => X.FirstName).HasMaxLength(50);
                OEntity.Property(X => X.LastName).HasMaxLength(50);
                OEntity.Property(X => X.City).HasMaxLength(50);
                OEntity.Property(X => X.Country).HasMaxLength(50);
                OEntity.Property(X => X.Street).HasMaxLength(50);
            });
        }
    }
}
