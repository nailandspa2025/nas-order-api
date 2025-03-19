using BuildingBlocks.Persistence.Abstractions.Auditing;

namespace BuildingBlocks.Persistence.Models;

public abstract class BaseAuditableDto : IAuditable
{
    public DateTime Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}

public abstract class BaseAuditableDto<T> : IAuditable
{
    public DateTime Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}
