using System.Linq.Expressions;

namespace Modsen.Portal.Core.Utils.Extensions;

public static class QueryableExtensions
{
    public static IOrderedQueryable<T> OrderByDirection<T, TKey>(
        this IQueryable<T> query,
        Expression<Func<T, TKey>> selector,
        bool descending)
    {
        return descending
            ? query.OrderByDescending(selector)
            : query.OrderBy(selector);
    }
}
