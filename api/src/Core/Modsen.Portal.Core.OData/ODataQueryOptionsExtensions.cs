using System.Globalization;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Core.OData;

public static class ODataQueryOptionsExtensions
{
    public static ODataQueryOptions<T> RebuildWithoutPagination<T>(this ODataQueryOptions<T> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Request.QueryString = options.RawValues.Filter.IsNullOrWhiteSpace()
            ? new QueryString()
            : new QueryString($"?filter={HttpUtility.UrlEncode(options.RawValues.Filter)}");

        return new ODataQueryOptions<T>(
            options.Context,
            options.Request);
    }

    public static ODataQueryOptions<T> RebuildWithSortingAndPagination<T>(this ODataQueryOptions<T> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var stringBuilder = new StringBuilder();

        if (!options.RawValues.OrderBy.IsNullOrWhiteSpace())
        {
            stringBuilder.Append(CultureInfo.InvariantCulture, $"$orderby={HttpUtility.UrlEncode(options.RawValues.OrderBy)}");
        } 

        if (!options.RawValues.Skip.IsNullOrWhiteSpace())
        {
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append('&');
            }

            stringBuilder.Append(CultureInfo.InvariantCulture, $"$skip={HttpUtility.UrlEncode(options.RawValues.Skip)}");
        }

        if (!options.RawValues.Top.IsNullOrWhiteSpace())
        {
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append('&');
            }

            stringBuilder.Append(CultureInfo.InvariantCulture, $"$top={HttpUtility.UrlEncode(options.RawValues.Top)}");
        }

        options.Request.QueryString = stringBuilder.Length > 0
            ? new QueryString($"?{stringBuilder}")
            : new QueryString();

        return new ODataQueryOptions<T>(options.Context, options.Request);
    }
}
