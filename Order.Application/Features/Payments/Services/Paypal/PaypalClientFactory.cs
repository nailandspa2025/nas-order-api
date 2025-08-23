using BuildingBlocks.ApiClients.Clients.Catalog;
using PayPalCheckoutSdk.Core;


namespace Order.Application.Features.Payments.Services.Paypal;

public static class PaypalClientFactory
{
    public static PayPalHttpClient CreateClient(PaypalConfigDto config)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));

        PayPalEnvironment environment = config.IsSandbox
            ? new SandboxEnvironment(config.ClientId, config.ClientSecret)
            : new LiveEnvironment(config.ClientId, config.ClientSecret);

        return new PayPalHttpClient(environment);
    }
}
