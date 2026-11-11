using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;

internal interface IHasUserActiveAssignmentForDateBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int projectId, int userId, DateOnly reportForDate,
        CancellationToken cancellationToken);
}

internal sealed class HasUserActiveAssignmentForDateBusinessRule : IHasUserActiveAssignmentForDateBusinessRule
{
    private readonly IEnvironmentFeatureContext _environmentFeatureContext;
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public HasUserActiveAssignmentForDateBusinessRule(IEnvironmentFeatureContext environmentFeatureContext,
        ITimeTrackDbContext timeTrackDbContext)
    {
        _environmentFeatureContext = environmentFeatureContext;
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(int projectId, int userId, DateOnly reportForDate,
        CancellationToken cancellationToken)
    {
        var checkAssignDates = await _environmentFeatureContext.IsCheckAssignDatesFeatureEnabledAsync();

        var isUserAbleToTrackTime = await _timeTrackDbContext.Projects.AnyAsync(project =>
                project.Id == projectId &&
                (project.IsPublic || !checkAssignDates ||
                    project.ProjectUsers.Any(projectUser =>
                        projectUser.UserId == userId &&
                        projectUser.UserOnProjectFrom <= reportForDate &&
                        (!projectUser.UserOnProjectTo.HasValue || reportForDate <= projectUser.UserOnProjectTo))),
            cancellationToken);

        return isUserAbleToTrackTime;
    }
}
