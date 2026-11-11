using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients;

internal interface IGetPageableClientsDbQuery
        : IDbQuery<GetPageableClientsDbRequest, PageableListOfItems<PageableClientDbModel>>;

internal record GetPageableClientsDbRequest(
    ODataQueryOptions<OdpClient> QueryOptions,
    string? DepartmentId,
    int? ClientHasProjectWithUsersFromResourceGroupId,
    int? ClientHasProjectCreatedByUserId);

internal record PageableClientDbModel(
    int Id,
    string Name,
    string? Color,
    string? Comments,
    int FactTrackedTimeInMinutes,
    int BillableTrackedTimeInMinutes,
    bool IsActive,
    bool IsProjectExist,
    bool HasActiveProject,
    string? RegisteredLocation,
    int? SalesManagerId);
