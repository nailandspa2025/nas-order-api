using BuildingBlocks.Authentication;
using BuildingBlocks.Common.API.Versioning;
using BuildingBlocks.Common.API.Error;
using BuildingBlocks.Common.Logging;
using BuildingBlocks.Common.Swagger;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZymLabs.NSwag.FluentValidation;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Common.Extensions;

public static class CommonExtensions
{
    public static IServiceCollection AddDefaultAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomAuthentication(configuration);
        services.AddCustomVersioning();

        services.AddVersionedApiExplorer(options =>
        {
            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            options.GroupNameFormat = "'v'VVV";

            // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
            // can also be used to control the format of the API version in route templates
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddScoped<FluentValidationSchemaProcessor>(provider =>
        {
            var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
            var loggerFactory = provider.GetService<ILoggerFactory>();

            return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
        });

        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        // services.AddFluentValidationAutoValidation();
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddScoped<ApplicationErrorDescriber>();

        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

        return services;
    }

    public static WebApplication UseServiceDefaults(this WebApplication app, WebApplicationBuilder builder)
    {
        var loggerFactory = app.Services.GetService<ILoggerFactory>();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
            loggerFactory?.AddFileSerilog(builder.Configuration, builder.Environment.ContentRootPath);
        }
        else
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            // loggerFactory?.AddAmazonCloudSerilog(builder.Configuration, builder.Environment);
            loggerFactory?.AddGoogleCloudSerilog(builder.Configuration);
        }

        app.UseHealthChecks("/health");
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseCustomSwagger();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        //app.UseMiddleware<SerilogMiddleware>();

        app.MapDefaultControllerRoute();

        app.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/liveness", new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });

        return app;
    }
}

