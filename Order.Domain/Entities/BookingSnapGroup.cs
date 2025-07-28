using BuildingBlocks.Persistence.Entities.Common;

namespace Order.Domain.Entities;

public class BookingSnapGroup : BaseAuditableEntity<int>
{
    public long BookingId { get; set; }
    public string GroupdId { get; set; } = null!;
    public Booking Booking { get; set; } = null!;
}
