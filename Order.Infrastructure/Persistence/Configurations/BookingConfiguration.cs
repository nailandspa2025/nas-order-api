using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Entities;

namespace Order.Infrastructure.Persistence.Configurations;

public class BookingConfiguration: IEntityTypeConfiguration<Booking>
{

    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(p => p.FullName)
             .HasMaxLength(100)
             .IsRequired();

        builder.Property(p => p.Phone)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Email)
            .HasMaxLength(50);

        builder.Property(p => p.Address)
            .HasMaxLength(250);

        builder.Property(p => p.Note)
            .HasMaxLength(250);

        builder.Property(p => p.Reason)
            .HasMaxLength(250);

        builder.Property(p => p.BookingDate)
            .HasColumnType("date");

        builder.HasMany(x => x.BookingTechnicians)
            .WithOne(x => x.Booking)
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.BookingProducts)
            .WithOne(x => x.Booking)
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        
        builder.HasMany(x => x.BookingColors)
            .WithOne(x => x.Booking)
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

