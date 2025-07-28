using BuildingBlocks.Persistence.Entities.Common;

namespace Order.Domain.Entities;

public class BookingSnap : BaseAuditableEntity<int>
{
    public long BookingId { get; set; }
    public string SnapId { get; set; } = null!;
    public Booking Booking { get; set; } = null!;
}
