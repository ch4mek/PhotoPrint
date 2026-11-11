using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

internal interface IGetTimeTrackUserByIdWithDepartmentDbQuery : IDbQuery<int, GetTimeTrackUserByIdWithDepartmentDbQueryResponse>;

internal record GetTimeTrackUserByIdWithDepartmentDbQueryResponse(int? UserId, string? DepartmentId, int? ResourceGroupId,
    string? Position, bool? IsActive, int? CreatedById);
