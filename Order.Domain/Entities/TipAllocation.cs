
using BuildingBlocks.Persistence.Entities.Common;
using Order.Domain.Enums;

namespace Order.Domain.Entities;

public class TipAllocation : BaseAuditableEntity<int>
{
    public int PaymentId { get; set; }
    public Payment Payment { get; set; } = null!;
    // doanh thu của kỹ thuật viên
    public decimal TechnicianRevenue { get; set; }
    public decimal TipAmount { get; set; }
    public long TechnicianId { get; set; }
    public TipAllocationType AllocationType { get; set; }
}