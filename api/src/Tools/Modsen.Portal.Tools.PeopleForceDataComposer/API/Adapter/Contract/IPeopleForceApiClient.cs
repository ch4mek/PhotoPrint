using System.Text.Json;
using Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.PeopleForceApiModels;

namespace Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.Contract;

internal interface IPeopleForceApiClient
{
    Task<ListOfEmployeesApiModel> GetEmployeesAsync(
        int page, string status,
        CancellationToken cancellationToken);

    Task<JsonDocument> GetByUriAsync(
      string uri, int? page,
      CancellationToken cancellationToken);

    Task<byte[]> GetFileByUriAsync(Uri uri, CancellationToken cancellationToken);
}
