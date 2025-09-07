using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Common.Firebase;
using BuildingBlocks.Common.Swagger;
using BuildingBlocks.CommonAuthorization.CommonAuthorizationExtensions;
using BuildingBlocks.EventBus;
using Loyalty.Infrastructure.Persistence;

namespace Loyalty.Api;

public static class ConfigureServices
{
    public static IServiceCollection AddAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultAPIServices(configuration);

        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddCustomSwagger(new Version[]
        {
            new Version(1, 0, 0)
        }, nameof(Loyalty));

        services.AddHealthChecks()
            .AddDbContextCheck<LoyaltyDbContext>();

        services.AddControllers();
        services.AddCloudinaryProvider(configuration);
        services.AddFirebaseProvider(configuration);
        services.AddEventServices(typeof(Program).Assembly, configuration);
        services.AddCommonAuthorization();
        return services;
    }
}

