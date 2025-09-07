using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;
using Loyalty.Domain.Enums;

namespace Loyalty.Domain.Entities;

public class LoyaltyConfigTier: BaseAuditableEntity<int>, ISoftDelete
{
    public string Name { get; set; } = null!;
    public int LoyaltyProgramId { get; set; }
    public LoyaltyProgram LoyaltyProgram { get; set; } = null!;

    public int LoyaltyTierId { get; set; }
    public LoyaltyTier LoyaltyTier { get; set; } = null!;
    public LoyaltyProcess Process { get; set; } = LoyaltyProcess.Payment;

    public decimal ThresholdAmount { get; set; }

    public bool IsActive { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }
}
