using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects.GetWithTotalTrackedTime;

public interface IGetProjectsWithTotalTrackedTimeDbQuery
    : IDbQuery<GetProjectsWithTotalTrackedTimeDbRequest, List<ProjectWithTotalTrackedTimeModel>>;
