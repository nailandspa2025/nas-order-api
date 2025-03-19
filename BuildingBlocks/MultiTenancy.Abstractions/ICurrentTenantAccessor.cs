namespace BuildingBlocks.MultiTenancy.Abstractions;

public interface ICurrentTenantAccessor
{
    BasicTenantInfo? Current { get; set; }
}

