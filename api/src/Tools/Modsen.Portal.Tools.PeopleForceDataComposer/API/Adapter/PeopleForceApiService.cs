using System.Text.Json;
using Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.Contract;
using Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.Mapping;
using Modsen.Portal.Tools.PeopleForceDataComposer.API.Contract;

namespace Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter;

internal sealed class PeopleForceApiService : IPeopleForceApiService
{
    private readonly IPeopleForceApiClient _peopleForceApiClients;

    public PeopleForceApiService(IPeopleForceApiClient peopleForceApiClient)
    {
        _peopleForceApiClients = peopleForceApiClient;
    }

    public Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken cancellationToken)
    {
        return GetEmployeesAsync("all", cancellationToken);
    }

    private async Task<List<EmployeeModel>> GetEmployeesAsync(string status, CancellationToken cancellationToken)
    {
        var employeesList = new List<EmployeeModel>();
        var employeesFirstPage = await _peopleForceApiClients.GetEmployeesAsync(1, status, cancellationToken);

        employeesList.AddRange(employeesFirstPage.Employees.Select(x => x.MapEmployee()));

        var pages = employeesFirstPage.Metadata!.Pagination!.Pages;
        for (var i = 2; i <= pages; i++)
        {
            var page = await _peopleForceApiClients.GetEmployeesAsync(i, status, cancellationToken);
            employeesList.AddRange(page.Employees.Select(x => x.MapEmployee()));
        }

        return employeesList;
    }

    public async Task<List<JsonElement>> GetDataAsJsonArrayByUri(
        string uri,
        CancellationToken cancellationToken,
        bool needPagination = true)
    {
        var dataFirstPage = await _peopleForceApiClients.GetByUriAsync(uri, 1, cancellationToken);
        var jsonRootFirst = dataFirstPage.RootElement;
        var allData = new List<JsonElement>();

        if (jsonRootFirst.TryGetProperty("data", out var dataFirst) && dataFirst.GetArrayLength() > 0)
        {
            foreach (var item in dataFirst.EnumerateArray())
            {
                allData.Add(item);
            }
        }

        if (needPagination)
        {
            var pages = GetPages(jsonRootFirst, uri);
            for (var i = 2; i <= pages; i++)
            {
                var page = await _peopleForceApiClients.GetByUriAsync(uri, i, cancellationToken);
                var jsonRoot = page.RootElement;

                if (jsonRoot.TryGetProperty("data", out var data) && data.GetArrayLength() > 0)
                {
                    foreach (var item in data.EnumerateArray())
                    {
                        allData.Add(item);
                    }
                }

                await Task.Delay(250, cancellationToken);
            }
        }

        return allData;
    }

    public async Task<JsonElement> GetDataAsJsonElementByUri(
        string uri,
        CancellationToken cancellationToken)
    {
        var data = await _peopleForceApiClients.GetByUriAsync(uri, null, cancellationToken);
        return data.RootElement;
    }

    private static int GetPages(JsonElement rootElement, string uri)
    {
        return
            GetPagesFromMetaPages(rootElement) ??
            GetPagesFromMetaPaginationPages(rootElement) ??
            1;
    }

    private static int? GetPagesFromMetaPages(JsonElement rootElement)
    {
        if (rootElement.TryGetProperty("metadata", out var metadata) &&
            metadata.TryGetProperty("pages", out var pagesString) &&
            pagesString.TryGetInt32(out var pages))
        {
            return pages;
        }

        return null;
    }

    private static int? GetPagesFromMetaPaginationPages(JsonElement rootElement)
    {
        if (rootElement.TryGetProperty("metadata", out var metadata) &&
            metadata.TryGetProperty("pagination", out var pagination) &&
            pagination.TryGetProperty("pages", out var pagesString) &&
            pagesString.TryGetInt32(out var pages))
        {
            return pages;
        }

        return null;
    }

    public Task<byte[]> GetFileContentById(Uri uri, CancellationToken cancellationToken)
    {
        return _peopleForceApiClients.GetFileByUriAsync(uri, cancellationToken);
    }
}
