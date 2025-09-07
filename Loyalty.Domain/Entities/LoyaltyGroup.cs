using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Loyalty.Domain.Entities;

public class LoyaltyGroup: BaseAuditableEntity<int>, ISoftDelete
{
    public String Name { get; set; } = null!;
    public int MerchantId { get; set; }
    public ICollection<LoyaltyTier> LoyaltyTiers { get; set; } = new List<LoyaltyTier>();
    public bool IsDraft { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? Deleted { get; set; }
    public bool IsDeleted { get; set; }
}
