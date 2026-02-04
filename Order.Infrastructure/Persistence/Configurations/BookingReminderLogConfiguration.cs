using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Entities;

namespace Order.Infrastructure.Persistence.Configurations;
public class BookingReminderLogConfiguration: IEntityTypeConfiguration<BookingReminderLog>
{

    public void Configure(EntityTypeBuilder<BookingReminderLog> builder)
    {
        builder.HasIndex(x => new { x.BookingId, x.ReminderConfigId })
           .IsUnique(); // ⛔ chống gửi trùng tuyệt đối

        builder.Property(x => x.Channel)
           .IsRequired();

        builder.Property(x => x.SentAt)
           .IsRequired();

        builder.HasOne(x => x.ReminderConfig)
           .WithMany()
           .HasForeignKey(x => x.ReminderConfigId)
           .OnDelete(DeleteBehavior.Restrict);
           
        builder.HasOne(x => x.Booking)
           .WithMany()
           .HasForeignKey(x => x.BookingId)
           .OnDelete(DeleteBehavior.Restrict);
    }
}
