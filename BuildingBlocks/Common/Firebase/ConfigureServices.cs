using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Common.Firebase;

public static class ConfigureServices
{
    public static IServiceCollection AddFirebaseProvider(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFirebaseService, FirebaseService>(); 
        return services;
    }
}