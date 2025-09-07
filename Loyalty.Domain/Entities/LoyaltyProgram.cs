using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Loyalty.Domain.Entities;

public class LoyaltyProgram : BaseAuditableEntity<int>, ISoftDelete
{
    public String Name { get; set; } = null!;
    public ICollection<LoyaltySetting> LoyaltySettings { get; set; } = new List<LoyaltySetting>();
    public ICollection<LoyaltyConfigTier> LoyaltyConfigTiers { get; set; } = new List<LoyaltyConfigTier>();
    public ICollection<LoyaltyConfigPoint> LoyaltyConfigPoints { get; set; } = new List<LoyaltyConfigPoint>();

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsDraft { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? Deleted { get; set; }
    public bool IsDeleted { get; set; }
}
