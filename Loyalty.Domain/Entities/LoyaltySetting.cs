using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Loyalty.Domain.Entities;

public class LoyaltySetting : BaseAuditableEntity<int>, ISoftDelete
{
    public int MerchantId { get; set; }
    public String Name { get; set; } = null!;
    public bool IsDraft { get; set; }
    public int LoyaltyProgramId { get; set; }
    public LoyaltyProgram? LoyaltyProgram { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? Deleted { get; set; }
    public bool IsDeleted { get; set; }
}
