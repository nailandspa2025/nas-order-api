using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Entities;

namespace Order.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration: IEntityTypeConfiguration<Payment>
{
	
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasOne(x=>x.Booking)
            .WithMany(x=>x.Payments)
            .HasForeignKey(p => p.BookingId)        
            .OnDelete(DeleteBehavior.Cascade);
    }
}

