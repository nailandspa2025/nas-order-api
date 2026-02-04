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
        var httpContext = _accessor.HttpContext;

        // Background job → không có HttpContext
        if (httpContext == null)
            return await base.SendAsync(request, cancellationToken);

        if (httpContext.Request.Headers.TryGetValue(AuthorizationHeader, out var values))
        {
            var authorizationHeader = values.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(authorizationHeader))
            {
                var token = authorizationHeader.Replace(
                    $"{BearerAuthenticationPrefix} ",
                    string.Empty
                );

                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }

}

