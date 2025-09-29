using BuildingBlocks.Persistence.Entities.Common;

namespace Order.Domain.Entities;

public class BookingSnapGroup : BaseAuditableEntity<int>
{
    public string GroupdId { get; set; } = null!;
    public Booking Booking { get; set; } = null!;
}
