using Order.Application.Common.Interfaces;
using Order.Infrastructure.Persistence;
using Order.Infrastructure.Services;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Persistence;
using BuildingBlocks.Persistence.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddCustomDbContext<OrderDbContext>(configuration, EfCoreDatabaseProvider.SqlServer);
        }

        services.AddScoped<IOrderDbContext>(provider => provider.GetRequiredService<OrderDbContext>());
        services.AddScoped<OrderDbContextInitialiser>();

        services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }
}

