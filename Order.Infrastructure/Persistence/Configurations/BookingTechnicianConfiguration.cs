using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Entities;

namespace Order.Infrastructure.Persistence.Configurations;

public class BookingTechnicianConfiguration: IEntityTypeConfiguration<BookingTechnicianService>
{

    public void Configure(EntityTypeBuilder<BookingTechnicianService> builder)
    {
       builder.HasOne(x => x.BookingTechnician)
            .WithMany(x => x.Services)
            .HasForeignKey(x => x.BookingTechnicianId);
    }
}

