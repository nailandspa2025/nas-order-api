using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace BuildingBlocks.ApiClients.Extensions;

public static class ServiceExtension
{
    public static Uri GetUpstreamMsUrl(IConfiguration configuration, string endpoint)
    {
        var url = configuration.GetSection("MicroserviceUri")[endpoint];

        if (string.IsNullOrEmpty(url) || !Uri.TryCreate(url.TrimEnd('/'), UriKind.RelativeOrAbsolute, out var uri))
            throw new ArgumentException($"Could not find valid url for {endpoint} ({configuration[endpoint]}");

        return uri;
    }

    public static IHttpClientBuilder AddRefitClient<T>(this IServiceCollection services, IConfiguration configuration, string uriConfigKey, RefitSettings? settings = null) where T : class
    {
        var address = configuration.GetSection("MicroserviceUris").GetValue<Uri>(uriConfigKey)
            ?? throw new ArgumentException($"Could not find valid url for {uriConfigKey} ({configuration[uriConfigKey]}");

        return services.AddRefitClient<T>(settings ?? new RefitSettings())
        .ConfigureHttpClient((provider, client) =>
        {
            if (!address.AbsoluteUri.EndsWith('/'))
            {
                address = new Uri($"{address.AbsoluteUri}/");
            }
            client.BaseAddress = address;
            client.Timeout = TimeSpan.FromSeconds(120);
        }).AddHttpMessageHandler<AuthorizationMessageHandler>();
    }
}

