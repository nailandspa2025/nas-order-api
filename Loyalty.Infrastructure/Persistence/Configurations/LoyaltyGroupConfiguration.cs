using Loyalty.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyalty.Infrastructure.Persistence.Configurations;

public class LoyaltyGroupConfiguration : IEntityTypeConfiguration<LoyaltyGroup>
{
    public void Configure(EntityTypeBuilder<LoyaltyGroup> builder)
    {
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(p => p.LoyaltyTiers)
            .WithOne(p => p.LoyaltyGroup)
            .HasForeignKey(p => p.LoyaltyGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
