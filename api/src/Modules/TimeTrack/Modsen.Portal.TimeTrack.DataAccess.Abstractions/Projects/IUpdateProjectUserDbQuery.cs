using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

internal interface IUpdateProjectUserDbQuery : IDbQuery<UpdateProjectUserDbRequest, UpdateProjectUserDbResponse>;

internal record UpdateProjectUserDbRequest(
    int Id,
    int UserId,
    int ProjectId,
    bool IsTimeOnProjectBillable,
    decimal Fte,
    DateOnly UserOnProjectFrom,
    DateOnly? UserOnProjectTo);

internal record UpdateProjectUserDbResponse(int Id,
    int UserId,
    int ProjectId);
