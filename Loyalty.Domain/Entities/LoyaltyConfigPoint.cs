using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;
using Loyalty.Domain.Enums;

namespace Loyalty.Domain.Entities;

public class LoyaltyConfigPoint : BaseAuditableEntity<int>, ISoftDelete
{
    public string Name { get; set; } = null!;

    public int LoyaltyProgramId { get; set; }
    public LoyaltyProgram LoyaltyProgram { get; set; } = null!;

    public int? LoyaltyTierId { get; set; }
    public LoyaltyTier? LoyaltyTier { get; set; }

    public int? LoyaltyGroupId { get; set; }
    public LoyaltyGroup? LoyaltyGroup { get; set; }

    public decimal AmountPerPoint { get; set; } = 1;
    public decimal PointValue { get; set; } = 1;
    public bool IsActive { get; set; }
    public RoundingRule RoundingRule { get; set; }

    public string? DeletedBy { get; set; }
    public DateTime? Deleted { get; set; }
    public bool IsDeleted { get; set; }
}
