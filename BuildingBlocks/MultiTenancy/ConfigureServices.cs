using BuildingBlocks.MultiTenancy.Abstractions;
using BuildingBlocks.MultiTenancy.ConfigurationStore;
using BuildingBlocks.MultiTenancy.TenantResolveContributors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace BuildingBlocks.MultiTenancy;

public static class ConfigureServices
{
    public static void AddCustomMultiTenancy(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TenantOptions>(configuration.GetSection(nameof(TenantOptions)));

        services.AddScoped<ITenantResolveContributor, HeaderTenantResolveContributor>();
        services.AddScoped<ITenantResolveContributor, JwtTokenTenantResolveContributor>();
        services.AddScoped<ITenantResolveContributor, QueryStringTenantResolveContributor>();
        services.AddScoped<ITenantResolveContributor, RouteValueTenantResolveContributor>();

        services.AddScoped<ITenantResolver, TenantResolver>();
        services.AddScoped<ITenantStore, DefaultTenantStore>();

        services.AddSingleton<ICurrentTenantAccessor>(AsyncLocalCurrentTenantAccessor.Instance);
    }
}

