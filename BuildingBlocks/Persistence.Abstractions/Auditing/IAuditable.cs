namespace BuildingBlocks.Persistence.Abstractions.Auditing;

public interface IAuditable
{
    DateTime Created { get; set; }

    string? CreatedBy { get; set; }

    DateTime? LastModified { get; set; }

    string? LastModifiedBy { get; set; }
}

