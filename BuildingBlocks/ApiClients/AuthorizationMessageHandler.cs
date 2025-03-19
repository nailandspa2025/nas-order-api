using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.ApiClients;

public class AuthorizationMessageHandler: DelegatingHandler
{
    private const string AuthorizationHeader = "Authorization";
    private const string BearerAuthenticationPrefix = "Bearer";
    private readonly IHttpContextAccessor _accessor;

    public AuthorizationMessageHandler(IHttpContextAccessor accessor)
    {
        _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // get the token
        var requestData = _accessor.HttpContext?.Request;
        if (requestData!.Headers.TryGetValue(AuthorizationHeader, out var values))
        {
            var authHeaders = values.ToList();
            if (authHeaders.Any())
            {
                var authorizationHeader = authHeaders.First();
                var token = authorizationHeader.Replace($"{BearerAuthenticationPrefix} ", string.Empty);

                // add header
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

