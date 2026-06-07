using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Entities;

namespace Order.Infrastructure.Persistence.Configurations;

public class PaymentProviderConfiguration : IEntityTypeConfiguration<PaymentProvider>
{
    public void Configure(EntityTypeBuilder<PaymentProvider> builder)
    {
        builder.HasMany(x => x.PaymentProviderSettings)
        .WithOne(x => x.PaymentProvider)
        .HasForeignKey(x => x.PaymentProviderId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}