
using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;
namespace Order.Domain.Entities;

public class ReminderConfig : BaseAuditableEntity<int>, ISoftDelete
{

    public string Name { get; set; }

    public int StoreId { get; set; }

    public ReminderChannel Channel { get; set; }

    public int BeforeMinute { get; set; }

    public bool IsActive { get; set; }
     public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }
}