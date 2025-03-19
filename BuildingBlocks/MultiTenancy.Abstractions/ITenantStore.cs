namespace BuildingBlocks.MultiTenancy.Abstractions;

public interface ITenantStore
{
    Task<TenantConfiguration[]> FindAllAsync();

    Task<TenantConfiguration?> FindAsync(string name);

    Task<TenantConfiguration?> FindAsync(Guid id);
}