using System.Text.Json;

namespace Modsen.Portal.Tools.PeopleForceDataComposer.API.Contract;

public interface IPeopleForceApiService
{
    Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken cancellationToken);

    Task<List<JsonElement>> GetDataAsJsonArrayByUri(string uri, CancellationToken cancellationToken, bool needPagination = true);

    Task<JsonElement> GetDataAsJsonElementByUri(string uri, CancellationToken cancellationToken);

    Task<byte[]> GetFileContentById(Uri uri, CancellationToken cancellationToken);
}
