using System.Linq.Expressions;

namespace SqlRepositoryBase.Core.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> queryable,
        bool condition,
        Expression<Func<T, bool>> expression
    )
    {
        return condition ? queryable.Where(expression) : queryable;
    }
}