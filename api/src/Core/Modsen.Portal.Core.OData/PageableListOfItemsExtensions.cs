using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.Core.OData;

public static class PageableListOfItemsExtensions
{
    public static PageableListOfItems<T> WithPaging<T, TEntity>(this PageableListOfItems<T> listOfItems, ODataQueryOptions<TEntity> options)
    {
        ArgumentNullException.ThrowIfNull(listOfItems);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Top);
        ArgumentNullException.ThrowIfNull(options.Skip);

        var top = options.Top!.Value;

        listOfItems.WithPaging((options.Skip!.Value / top) + 1, top);

        return listOfItems;
    }

    public static PageableListOfItems<T> ToPageable<T, TEntity>(this IReadOnlyCollection<T> listOfItems, ODataQueryOptions<TEntity> options, int total)
    {
        ArgumentNullException.ThrowIfNull(listOfItems);

        if (total == 0)
        {
            return EmptyPageableListOfItems<T>.Value;
        }

        ArgumentNullException.ThrowIfNull(options);

        return listOfItems.ToPageable().WithPaging(options).WithTotal(total);
    }

    public static PageableListOfItems<T> ToPageable<T, TEntity>(this IEnumerable<T> listOfItems, ODataQueryOptions<TEntity> options, int total)
    {
        ArgumentNullException.ThrowIfNull(listOfItems);
        return listOfItems.ToArray().ToPageable(options, total);
    }
}
