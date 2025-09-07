using Loyalty.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyalty.Infrastructure.Persistence.Configurations;

public class LoyaltyConfigPointConfiguration : IEntityTypeConfiguration<LoyaltyConfigPoint>
{
    public void Configure(EntityTypeBuilder<LoyaltyConfigPoint> builder)
    {
        builder.Property(p => p.Name)
          .HasMaxLength(100)
          .IsRequired();

        builder.HasOne(p => p.LoyaltyProgram)
            .WithMany(p => p.LoyaltyConfigPoints)
            .HasForeignKey(p => p.LoyaltyProgramId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
