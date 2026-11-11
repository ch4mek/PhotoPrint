using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;
using Modsen.Portal.Finance.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Finance;

internal static class HttpClientManagementsExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<HttpResponseMessage> GetManagementsAsResponseMessageAsync(this HttpClient httpClient,
        int year, int month,
        GetManagementsInput input)
    {
        var queryParams = new List<KeyValuePair<string, string?>>
    {
        new("userId", input.UserId?.ToString(CultureInfo.InvariantCulture)),
        new("userFullName", input.UserFullName),
        new("projectName", input.ProjectName),
        new("clientName", input.ClientName),
        new("clientLegalEntity", input.ClientLegalEntity),
        new("pageSize", input.PageSize?.ToString(CultureInfo.InvariantCulture)),
        new("pageNumber", input.PageNumber?.ToString(CultureInfo.InvariantCulture)),
        new("orderingField", input.OrderingField?.ToString()),
        new("isDescendingOrdering", input.IsDescendingOrdering?.ToString()),
        new("shouldCalculateMainStatistics", input.ShouldCalculateMainStatistics.ToString()),
        new("shouldCalculateColumnStatistics", input.ShouldCalculateColumnStatistics.ToString())
    };

        if (input.DepartmentIds is not null)
        {
            foreach (var departmentId in input.DepartmentIds)
            {
                queryParams.Add(new("departmentIds", departmentId));
            }
        }

        var url = QueryHelpers.AddQueryString(
            $"api/managements/{year}/{month}",
            queryParams);

        return httpClient.GetAsync(new Uri(url, UriKind.Relative), _cancellationToken);
    }
}
