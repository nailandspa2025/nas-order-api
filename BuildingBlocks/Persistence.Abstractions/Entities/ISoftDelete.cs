namespace BuildingBlocks.Persistence.Abstractions.Entities;

public interface ISoftDelete
{
    string? DeletedBy { get; set; }

    DateTime? Deleted { get; set; }

    bool IsDeleted { get; set; }
}

