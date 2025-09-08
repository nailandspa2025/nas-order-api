using Loyalty.Application.Common.Interfaces;
using Loyalty.Infrastructure.Persistence;
using Loyalty.Infrastructure.Services;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Persistence;
using BuildingBlocks.Persistence.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.ApiClients.Extensions;
using BuildingBlocks.ApiClients.Clients.Identity;
using Refit;
using BuildingBlocks.ApiClients;
using BuildingBlocks.ApiClients.Clients.Catalog;

namespace Loyalty.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.Is("UseInMemoryDatabase"))
        {
            services.AddCustomDbContext<LoyaltyDbContext>(configuration, EfCoreDatabaseProvider.InMemory);
        }
        else
        {
            services.AddCustomDbContext<LoyaltyDbContext>(configuration, EfCoreDatabaseProvider.PostgreSql);
        }

        services.AddScoped<ILoyaltyDbContext>(provider => provider.GetRequiredService<LoyaltyDbContext>());
        services.AddScoped<OrderDbContextInitialiser>();

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddScoped<AuthorizationMessageHandler>();
        services.AddRefitClient<IIdentityClient>()
               .ConfigureHttpClient((sp, client) => client.BaseAddress = configuration
                                                                       .GetSection("MicroserviceUri")
                                                                       .GetValue<Uri>("IdentityAddress"))
               .AddHttpMessageHandler<AuthorizationMessageHandler>();

        services.AddRefitClient<ICatalogClient>()
               .ConfigureHttpClient((sp, client) => client.BaseAddress = configuration
                                                                       .GetSection("MicroserviceUri")
                                                                       .GetValue<Uri>("CatalogAddress"))
               .AddHttpMessageHandler<AuthorizationMessageHandler>();

        return services;
    }
}

