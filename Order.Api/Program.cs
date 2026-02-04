using Order.Api;
using Order.Application;
using Order.Infrastructure;
using Order.Infrastructure.Persistence;
using BuildingBlocks.Common.Extensions;
using Hangfire;
using Hangfire.PostgreSql;
using Order.Api.JobHangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql.Properties;


var builder = WebApplication.CreateBuilder(args);

//builder.Host.UseSerilog()
//    .ConfigureAppConfiguration(AppConfiguration.Configure);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddAPIServices(builder.Configuration);
builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(
        builder.Configuration.GetConnectionString("HangfireConnection"),
        new PostgreSqlStorageOptions
        {
            SchemaName = "hangfire",
            PrepareSchemaIfNecessary = true,
            QueuePollInterval = TimeSpan.FromSeconds(15)
        }
    );
});
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = Environment.ProcessorCount * 2;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseServiceDefaults(builder);

await app.InitialiseDatabaseAsync();

app.UseRouting();

app.UseHangfireServer();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new IDashboardAuthorizationFilter[]
    {
        new InlineAllowAllDashboardAuthorizationFilter()
    }
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


//RecurringJob.AddOrUpdate<BookingReminderJob>("booking-reminder-job", job => job.ExecuteAsync(), "0 0 * * *");
RecurringJob.AddOrUpdate<BookingReminderJob>("booking-reminder-job",job => job.ExecuteAsync(),Cron.Minutely);
app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }

class InlineAllowAllDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        return true;
    }
}


