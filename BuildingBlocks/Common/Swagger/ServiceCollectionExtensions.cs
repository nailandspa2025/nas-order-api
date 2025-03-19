

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.Processors.Security;
using ZymLabs.NSwag.FluentValidation;


namespace BuildingBlocks.Common.Swagger;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services,
        Version[] versions,
        string swaggerTitle)
    {
        foreach (var version in versions)
        {
            services.AddOpenApiDocument((configure, serviceProvider) =>
            {
                var fluentValidationSchemaProcessor = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<FluentValidationSchemaProcessor>();

                // Add the fluent validations schema processor
                //configure.SchemaProcessors.Add(fluentValidationSchemaProcessor);
                configure.Title = $"{swaggerTitle} API";
                configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });

                configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                configure.DocumentName = "v" + version.Major;
                configure.ApiGroupNames = new string[] { "v" + version.Major };
                configure.Version = version.Major + "." + version.Minor;
            });
        }

        return services;
    }

    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
    {
        app.UseOpenApi();
        app.UseSwaggerUi(e =>
        {
            e.Path = "/api";
        });

        return app;
    }
}


