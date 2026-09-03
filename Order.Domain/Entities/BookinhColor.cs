using BuildingBlocks.Persistence.Entities.Common;

namespace Order.Domain.Entities;

public class BookingColor : BaseAuditableEntity<int>
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public string HexColor { get; set; } = null!;
}