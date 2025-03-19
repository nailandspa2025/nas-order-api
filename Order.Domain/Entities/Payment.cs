using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;
namespace Order.Domain.Entities;

public class Payment : BaseAuditableEntity<int>, ISoftDelete
{

    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaidAt { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }
}

