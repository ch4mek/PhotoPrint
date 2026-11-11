namespace Modsen.Portal.Core.Utils;

public class PageableListOfItems<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public required IReadOnlyCollection<T> Items { get; set; } = [];
}

public static class PageableListOfItemsExtensions
{
    public static PageableListOfItems<T> ToPageable<T>(this IEnumerable<T> items)
    {
        return items.ToArray().ToPageable();
    }

    public static PageableListOfItems<T> ToPageable<T>(this IReadOnlyCollection<T> items)
    {
        return new PageableListOfItems<T> { Items = items };
    }

    public static PageableListOfItems<T> WithPaging<T>(this PageableListOfItems<T> listOfItems, int page, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(listOfItems);
        listOfItems.Page = page;
        listOfItems.PageSize = pageSize;

        return listOfItems;
    }
    
    public static PageableListOfItems<T> WithTotal<T>(this PageableListOfItems<T> listOfItems, int total)
    {
        ArgumentNullException.ThrowIfNull(listOfItems);
        listOfItems.Total = total;

        return listOfItems;
    }
}

public static class EmptyPageableListOfItems<T>
{
    public static readonly PageableListOfItems<T> Value = Array.Empty<T>().ToPageable();
}
