using E_commerce.Domain.Entities.IdentityModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce.Persistence.Data.Configurations
{
    /// <summary>Our extra columns on the Identity "Users" table.</summary>
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(X => X.DisplayName).HasMaxLength(100);
            builder.HasIndex(X => X.CreatedAt);
        }
    }
}
