using BuildingBlocksCommonAuthorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.CommonAuthorization.CommonAuthorizationExtensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddCommonAuthorization(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IAuthorizationService, ClaimAuthorizationService>();
        return services;
    }
}
