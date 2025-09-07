using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Loyalty.Domain.Entities;

public class LoyaltyTier: BaseAuditableEntity<int>, ISoftDelete
{
    public String Name { get; set; }

    public int Level { get; set; }

    public int LoyaltyGroupId { get; set; }
    public LoyaltyGroup LoyaltyGroup { get; set; }

    public bool IsActive { get; set; }
    public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }
}
