using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;


namespace BuildingBlocks.Common.API.Versioning;

public static class ApiVersionningExtensions
{
    public static void AddCustomVersioning(this IServiceCollection services,
        Action<ApiVersioningOptions>? configurator = null)
    {
        //https://www.meziantou.net/versioning-an-asp-net-core-api.htm
        //https://exceptionnotfound.net/overview-of-api-versioning-in-asp-net-core-3-0/
        services.AddApiVersioning(options =>
        {
            
            options.ReportApiVersions = true;
         
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);

            options.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("api-version"),
                new UrlSegmentApiVersionReader());

            configurator?.Invoke(options);
        });
    }
}

