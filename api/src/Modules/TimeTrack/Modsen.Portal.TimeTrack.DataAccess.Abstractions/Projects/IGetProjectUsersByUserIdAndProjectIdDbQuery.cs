using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

internal interface IGetProjectUsersByUserIdAndProjectIdDbQuery
    : IDbQuery<GetProjectUsersByUserIdAndProjectIdDbRequest, List<GetProjectUsersByUserIdAndProjectIdDbResponse>>;

internal record GetProjectUsersByUserIdAndProjectIdDbRequest(int Userid, int ProjectId);

internal record GetProjectUsersByUserIdAndProjectIdDbResponse(int Id, DateOnly UserOnProjectFrom, 
    DateOnly? UserOnProjectTo);
