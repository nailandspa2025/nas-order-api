namespace BuildingBlocks.MultiTenancy.Abstractions;

public interface ITenantResolveContributor
{
    public string Name { get; }

    public Task<string> GetTenantIdFromHttpContextOrEmptyAsync();
}
