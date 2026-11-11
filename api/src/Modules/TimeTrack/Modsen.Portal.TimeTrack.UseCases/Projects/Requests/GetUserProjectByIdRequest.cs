using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TimeTrack.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetUserProjectByIdRequest
    : IQueryHandler<GetUserProjectByIdRequest.Query, UserProjectWithNamesDto>
{
    private readonly ICanTimeTrackUserBeRetrievedWithSelfBusinessRule _canTimeTrackUserBeRetrievedWithSelfBusinessRule;
    private readonly IGetProjectUserByIdDbQuery _getProjectUserByIdDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly ITeamBoardContract _teamBoardContract;

    public GetUserProjectByIdRequest(
        ICanTimeTrackUserBeRetrievedWithSelfBusinessRule canTimeTrackUserBeRetrievedWithSelfBusinessRule,
        IGetProjectUserByIdDbQuery getProjectUserByIdDbQuery,
        IAuthenticatedUserContext authenticatedUserContext,
        ITimeTrackContract timeTrackContract,
        ITimeTrackDbContext timeTrackDbContext,
        ITeamBoardContract teamBoardContract)
    {
        _canTimeTrackUserBeRetrievedWithSelfBusinessRule = canTimeTrackUserBeRetrievedWithSelfBusinessRule;
        _getProjectUserByIdDbQuery = getProjectUserByIdDbQuery;
        _authenticatedUserContext = authenticatedUserContext;
        _timeTrackDbContext = timeTrackDbContext;
        _teamBoardContract = teamBoardContract;
    }

    public async Task<UserProjectWithNamesDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var projectUser = await _getProjectUserByIdDbQuery.ExecuteAsync(request.UserProjectId, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ProjectAssignNotFound);

        var isChief = _authenticatedUserContext.GetRoles().IsCto() || _authenticatedUserContext.GetRoles().IsCeo();
        if (isChief)
        {
            var canBeRetrievedResult = await _canTimeTrackUserBeRetrievedWithSelfBusinessRule.IsCorrespondsToBusiness(
                new CanTimeTrackUserBeRetrievedWithSelfBusinessRuleModel(projectUser.UserId), cancellationToken);

            if (!canBeRetrievedResult.IsSuccess)
            {
                throw new ActionNotAllowedException(PortalMessages.YouDoNotHaveAccessToUserProject);
            }
        }

        var isAssignHasTrackedTime = await _timeTrackDbContext.Timesheets
            .Where(timesheet => timesheet.UserId == projectUser.UserId)
            .Where(timesheet => timesheet.ProjectId == projectUser.ProjectId)
            .Where(timesheet => timesheet.ReportForDate >= projectUser.UserOnProjectFrom)
            .Where(timesheet => projectUser.UserOnProjectTo == null || timesheet.ReportForDate <= projectUser.UserOnProjectTo)
            .AnyAsync(cancellationToken);

        var seniorityLevel = await _teamBoardContract.GetSeniorityLevelByUserProjectId(projectUser.Id, cancellationToken);

        return new UserProjectWithNamesDto
        {
            Id = projectUser.Id,
            ProjectId = projectUser.ProjectId,
            UserId = projectUser.UserId,
            ProjectName = projectUser.ProjectName,
            UserFullName = projectUser.UserFullName,
            UserOnProjectFrom = projectUser.UserOnProjectFrom,
            UserOnProjectTo = projectUser.UserOnProjectTo,
            IsTimeOnProjectBillable = projectUser.IsTimeOnProjectBillable,
            Fte = projectUser.Fte,
            SeniorityLevel = seniorityLevel.Name,
            IsAssignEditable = !isAssignHasTrackedTime,
        };
    }

    internal sealed record Query(int UserProjectId) : IQuery<UserProjectWithNamesDto>;
}
