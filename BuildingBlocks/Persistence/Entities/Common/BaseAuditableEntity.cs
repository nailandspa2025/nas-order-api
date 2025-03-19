using BuildingBlocks.Persistence.Abstractions.Auditing;

namespace BuildingBlocks.Persistence.Entities.Common;

public abstract class BaseAuditableEntity: BaseEntity, IAuditable
{
    public DateTime Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}
public abstract class BaseAuditableEntity<T> : BaseEntity<T>, IAuditable
{
    public DateTime Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}

