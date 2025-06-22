using Order.Domain.Enums;
using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Order.Domain.Entities;

public class Notification : BaseAuditableEntity<int>, ISoftDelete
{
    public string? AccountId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }

    public int? BookingId { get; set; }

    public virtual Booking? Booking { get; set; } 
}
