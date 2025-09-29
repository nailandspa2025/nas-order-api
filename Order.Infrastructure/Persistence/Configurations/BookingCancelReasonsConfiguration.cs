using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Entities;

namespace Order.Infrastructure.Persistence.Configurations;

public class BookingCancelReasonsConfiguration : IEntityTypeConfiguration<BookingCancelReason>
{
    public void Configure(EntityTypeBuilder<BookingCancelReason> builder)
    {
        builder.Property(p => p.Name)
            .HasMaxLength(250)
            .IsRequired();

        builder.HasMany(x => x.Bookings)
            .WithOne(x => x.BookingCancelReason)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

