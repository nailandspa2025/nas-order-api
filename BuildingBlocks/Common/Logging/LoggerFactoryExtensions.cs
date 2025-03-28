using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Amazon.CloudWatchLogs;
using Serilog;
using Serilog.Sinks.AwsCloudWatch;
using Serilog.Sinks.GoogleCloudLogging;

namespace BuildingBlocks.Common.Logging;

public static class LoggerFactoryExtensions
{
    const string MessageTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message} LogProperties={Properties}{NewLine}{Exception}";

    public static void AddGoogleCloudSerilog(this ILoggerFactory loggerFactory, IConfiguration configuration, GoogleCloudLoggingSinkOptions sinkOptions = null)
    {
        var loggerBuilder = AddCommonLoggerConfiguration(configuration);

        if (sinkOptions == null)
        {
            string cloudRoleName = Assembly.GetEntryAssembly().GetName().Name;
            sinkOptions = new GoogleCloudLoggingSinkOptions
            {
                LogName = cloudRoleName,
                ServiceName = cloudRoleName,
                UseLogCorrelation = true,
                UseSourceContextAsLogName = false,
            };
        }

        loggerBuilder
           .WriteTo.GoogleCloudLogging(sinkOptions);

        Log.Logger = loggerBuilder.CreateLogger();
        loggerFactory.AddSerilog();
    }

    public static void AddAmazonCloudSerilog(this ILoggerFactory loggerFactory, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        var loggerBuilder = AddCommonLoggerConfiguration(configuration);
        var client = new AmazonCloudWatchLogsClient();
        string cloudRoleName = Assembly.GetEntryAssembly().GetName().Name;

        loggerBuilder
             .WriteTo.AmazonCloudWatch(
                // The name of the log group to log to
                logGroup: $"{cloudRoleName}_{webHostEnvironment.EnvironmentName}",
                // A string that our log stream names should be prefixed with. We are just specifying the
                // start timestamp as the log stream prefix
                logStreamPrefix: DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"),
                // The AWS CloudWatch client to use
                cloudWatchClient: client);

        Log.Logger = loggerBuilder.CreateLogger();
        loggerFactory.AddSerilog();
    }

    public static void AddFileSerilog(this ILoggerFactory loggerFactory, IConfiguration configuration, string contentRootPath)
    {
        var loggerBuilder = AddCommonLoggerConfiguration(configuration);

        loggerBuilder
            .WriteTo.File(path: Path.Combine($"{contentRootPath}\\logs", "log-.txt"),
            rollingInterval: RollingInterval.Day,
            outputTemplate: MessageTemplate)
            .WriteTo.Console(outputTemplate: MessageTemplate);

        Log.Logger = loggerBuilder.CreateLogger();
        loggerFactory.AddSerilog();
    }

    private static LoggerConfiguration AddCommonLoggerConfiguration(IConfiguration configuration)
        => new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                
                .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("/liveness")))
                .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("/hc")))
                .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("/swagger")))
                .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("/health")));
}
