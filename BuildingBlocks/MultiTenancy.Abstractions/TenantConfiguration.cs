namespace BuildingBlocks.MultiTenancy.Abstractions;

public class TenantConfiguration
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDefault()
    {
        return Id == Guid.Empty;
    }

    public string? ApiKey { get; set; }

    public string? ConnectionString { get; set; }

    public TenantConfiguration()
    {
        IsActive = true;
    }
}

