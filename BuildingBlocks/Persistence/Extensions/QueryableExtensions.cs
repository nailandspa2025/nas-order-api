using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> IncludeIf<T, TProperty>(this IQueryable<T> source, bool condition, Expression<Func<T, TProperty>> path)
        where T : class
    {
        return condition
            ? source.Include(path)
            : source;
    }
}