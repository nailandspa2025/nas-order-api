using BuildingBlocks.Persistence.Entities.Common;

namespace Order.Domain.Entities;

public class BookingProduct : BaseAuditableEntity<int>
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public long ProductId { get; set; }
}