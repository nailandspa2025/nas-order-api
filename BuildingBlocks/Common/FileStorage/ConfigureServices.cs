using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Common.FileStorage;

public static class ConfigureServices
{
    public static IServiceCollection AddS3StorageProvider(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IStorageService, S3StorageService>();
        return services;
    }

    public static IServiceCollection AddCloudinaryProvider(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IStorageService, CloudinaryStorageService>();
        return services;
    }
}
