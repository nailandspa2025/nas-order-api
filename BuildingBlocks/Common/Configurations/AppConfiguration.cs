using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Common.Configurations;

public static class AppConfiguration
{
    public static Action<HostBuilderContext, IConfigurationBuilder> Configure =>
       (context, options) =>
       {
           if (context.HostingEnvironment.IsDevelopment())
           {
               var sharedFolder = Path.Combine(context.HostingEnvironment.ContentRootPath, "..", "..", "..", "BuildingBlocks", "Common", "Configurations");

               options
                    .AddJsonFile(Path.Combine(sharedFolder, "sharedConfiguration.json"), optional: true, reloadOnChange: true)
                    .AddJsonFile(Path.Combine(sharedFolder, $"sharedConfiguration.{context.HostingEnvironment.EnvironmentName}.json"), optional: true, reloadOnChange: true);

               sharedFolder = Path.Combine(context.HostingEnvironment.ContentRootPath, "..", "..", "BuildingBlocks", "Common", "Configurations");

               options
                    .AddJsonFile(Path.Combine(sharedFolder, "sharedConfiguration.json"), optional: true, reloadOnChange: true)
                    .AddJsonFile(Path.Combine(sharedFolder, $"sharedConfiguration.{context.HostingEnvironment.EnvironmentName}.json"), optional: true, reloadOnChange: true);
           }
           else
           {
               options
                    .AddJsonFile("sharedConfiguration.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"sharedConfiguration.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
           }
       };
}

