using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

internal interface IGetProjectUserByIdDbQuery : IDbQuery<int, GetProjectUserByIdDbResponse?>;

internal record GetProjectUserByIdDbResponse(
    int Id,
    int UserId,
    int ProjectId,
    DateOnly UserOnProjectFrom,
    DateOnly? UserOnProjectTo,
    bool IsTimeOnProjectBillable,
    decimal Fte,
    string ProjectName,
    string ClientName,
    string UserFullName,
    bool IsActive);
