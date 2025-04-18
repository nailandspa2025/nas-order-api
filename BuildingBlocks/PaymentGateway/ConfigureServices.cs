using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.PaymentGateway;

public static class ConfigureServices
{
	public static IServiceCollection AddPaymentMomoProvider ( this IServiceCollection services)
	{
        return services;
    }

    public static IServiceCollection AddPaymentVNPayProvider(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddPaymentZalopayProvider(this IServiceCollection services)
    {
        return services;
    }
}
