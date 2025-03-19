using Order.Domain.Enums;
using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Order.Domain.Entities;

public class Notification : BaseAuditableEntity<int>, ISoftDelete
{
    public string UserId { get; set; } = null!;

    public string ? Content { get; set; }

    public DateTime SentTime { get; set; }

    public NotificationStatus Status { get; set; }

    public string? DeletedBy { get ; set ; }

    public DateTime? Deleted { get ; set ; }

    public bool IsDeleted { get ;set; }
}
