<<<<<<< HEAD:Loyalty.Infrastructure/ConfigureServices.cs
﻿using Loyalty.Application.Common.Interfaces;
using Loyalty.Infrastructure.Persistence;
using Loyalty.Infrastructure.Services;
=======
﻿using Order.Application.Common.Interfaces;
using Order.Infrastructure.Persistence;
using Order.Infrastructure.Services;
>>>>>>> parent of 126ebf4 (api loyalty):Order.Infrastructure/ConfigureServices.cs
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

namespace Order.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.Is("UseInMemoryDatabase"))
        {
            services.AddCustomDbContext<OrderDbContext>(configuration, EfCoreDatabaseProvider.InMemory);
        }
        else
        {
            services.AddCustomDbContext<OrderDbContext>(configuration, EfCoreDatabaseProvider.PostgreSql);
        }

        services.AddScoped<IOrderDbContext>(provider => provider.GetRequiredService<OrderDbContext>());
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

