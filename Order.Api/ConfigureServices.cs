using Order.Infrastructure.Persistence;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Common.Firebase;
using BuildingBlocks.Common.Swagger;
using BuildingBlocks.EventBus;
using BuildingBlocks.CommonAuthorization.CommonAuthorizationExtensions;
namespace Order.Api;

public static class ConfigureServices
{
    public static IServiceCollection AddAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultAPIServices(configuration);

        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddCustomSwagger(new Version[]
        {
            new Version(1, 0, 0)
        }, nameof(Order));

        services.AddHealthChecks()
            .AddDbContextCheck<OrderDbContext>();

        services.AddControllers();
        services.AddCloudinaryProvider(configuration);
        services.AddFirebaseProvider(configuration);
        services.AddEventServices(typeof(Program).Assembly, configuration);
        services.AddCommonAuthorization();
        return services;
    }
}

