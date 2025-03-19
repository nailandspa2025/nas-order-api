using BuildingBlocks.Persistence.EntityFrameworkCore;
using BuildingBlocks.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Persistence;

public static class ConfigureServices
{
    public static void AddCustomDbContext<TDbContext>(this IServiceCollection services,
        IConfiguration configuration,
        EfCoreDatabaseProvider efCoreDatabaseProvider) where TDbContext : EfCoreDbContext<TDbContext>
    {
        //services.AddCustomMultiTenancy(configuration);

        services.AddScoped<ISaveChangesInterceptor, AuditableEntitySaveChangesInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<TDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            switch (efCoreDatabaseProvider)
            {
                case EfCoreDatabaseProvider.SqlServer:
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        builder => builder.MigrationsAssembly(typeof(TDbContext).Assembly.FullName));
                    break;

                //case EfCoreDatabaseProvider.MySql:
                //    options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                //        ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection")),
                //        builder => builder.MigrationsAssembly(typeof(TDbContext).Assembly.FullName));
                //    break;

                case EfCoreDatabaseProvider.PostgreSql:
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                        builder => builder.MigrationsAssembly(typeof(TDbContext).Assembly.FullName));
                    break;

                case EfCoreDatabaseProvider.InMemory:
                    options.UseInMemoryDatabase(typeof(TDbContext).Name);
                    break;
            }
        });
    }
}

