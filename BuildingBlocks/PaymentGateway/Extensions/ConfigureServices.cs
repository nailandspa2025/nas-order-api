using BuildingBlocks.PaymentGateway.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.PaymentGateway.Extensions;

public static class ConfigureServices
{
    public static IServiceCollection AddPayPal(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PayPalSettings>(configuration.GetSection("PayPalSettings"));
        services.AddHttpClient<IPayPalService, PayPalService>();
        return services;
    }
}

