using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Requests.RequestDtos;

internal class PageableListOfItemsWithTotalUsed<T> : PageableListOfItems<T>
{
    public decimal? TotalUsed { get; set; }

    public static PageableListOfItemsWithTotalUsed<T> Of(PageableListOfItems<T> source, decimal? totalUsed = null)
    {
        return new PageableListOfItemsWithTotalUsed<T>
        {
            Page = source.Page,
            PageSize = source.PageSize,
            Total = source.Total,
            Items = source.Items,
            TotalUsed = totalUsed,
        };
    }
}
