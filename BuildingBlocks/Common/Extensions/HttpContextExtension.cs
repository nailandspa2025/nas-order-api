using System.Net;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Common.Extensions;

public static class HttpContextExtension
{
    public static string GetClientIpAddress(this HttpContext httpContext)
    {
        if (httpContext == null)
        {
            return string.Empty;
        }

        if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            string xForwardedForHeader = httpContext.Request.Headers["X-Forwarded-For"].ToString().Split(',').Select(s => s.Trim()).First(); //first because <client>, <proxy1>, <proxy2>
            return IPAddress.Parse(xForwardedForHeader).ToString();
        }
        else
        {
            return httpContext.Connection?.RemoteIpAddress?.ToString();
        }
    }
}

