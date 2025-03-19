
using System.Reflection;
using BuildingBlocks.EventBus.Events;
using BuildingBlocks.EventBus.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.EventBus;

public static class ConfigureServices
{
    public static void AddEventServices(this IServiceCollection services, Assembly assembly, IConfiguration configuration)
    {
        services.Configure<AwsBusOptions>(configuration.GetSection(nameof(AwsBusOptions)));
        services.AddScoped<IQueueService, QueueService>();
        services.AddScoped<ITopicService, TopicService>();

        services.AddMassTransit(x =>
        {
            x.AddConsumers(assembly);

            x.UsingAmazonSqs((context, cfg) =>
            {
                cfg.Host(configuration["AwsBusOptions:Region"], h =>
                {
                    h.AccessKey(configuration["AwsBusOptions:AccessKey"]);
                    h.SecretKey(configuration["AwsBusOptions:SecretKey"]);
                    // specify a scope for all topics
                    h.Scope(configuration["AwsBusOptions:TargetEnvironment"], true);
                });

                // additionally include the queues
                cfg.ConfigureEndpoints(context, new DefaultEndpointNameFormatter($"{configuration["AwsBusOptions:TargetEnvironment"]}-", false));
                cfg.Message<UserLoggedOutEvent>(e => e.SetEntityName("identity-user-logged-out-topic"));
            });

            x.SetKebabCaseEndpointNameFormatter();
        });
    }
}
