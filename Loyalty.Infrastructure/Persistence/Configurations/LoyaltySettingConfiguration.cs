using Microsoft.EntityFrameworkCore;
using Loyalty.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyalty.Infrastructure.Persistence.Configurations;

public class LoyaltySettingConfiguration : IEntityTypeConfiguration<LoyaltySetting>
{
    public void Configure(EntityTypeBuilder<LoyaltySetting> builder)
    {
        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(p => p.LoyaltyProgram)
            .WithMany(p => p.LoyaltySettings)
            .HasForeignKey(p => p.LoyaltyProgramId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
