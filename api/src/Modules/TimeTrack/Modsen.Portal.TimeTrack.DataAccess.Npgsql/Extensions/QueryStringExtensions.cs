using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Extensions;

public static class QueryStringExtensions
{
    public static (DateOnly? startDate, DateOnly? endDate) ExtractDatesFromQueryString(string filter)
    {
        if (filter.IsNullOrEmpty())
        {
            return (null, null);
        }

        var decodedFilter = HttpUtility.UrlDecode(filter);

        var startDateMatch = Regex.Match(decodedFilter, @"reportForDate\s+ge\s+(\d{4}-\d{2}-\d{2})", RegexOptions.None, TimeSpan.FromSeconds(1));
        var endDateMatch = Regex.Match(decodedFilter, @"reportForDate\s+le\s+(\d{4}-\d{2}-\d{2})", RegexOptions.None, TimeSpan.FromSeconds(1));

        DateOnly? startDate = null;
        DateOnly? endDate = null;

        if (startDateMatch is { Success: true, Groups.Count: >= 2 })
        {
            startDate = DateOnly.Parse(startDateMatch.Groups[1].Value, CultureInfo.InvariantCulture);
        }

        if (endDateMatch is { Success: true, Groups.Count: >= 2 })
        {
            endDate = DateOnly.Parse(endDateMatch.Groups[1].Value, CultureInfo.InvariantCulture);
        }

        return (startDate, endDate);
    }
}
