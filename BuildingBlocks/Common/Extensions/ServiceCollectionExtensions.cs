using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAllTransients<T>(this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();

        foreach (var type in types)
        {
            services.AddTransient(typeof(T), type);
        }
    }
}