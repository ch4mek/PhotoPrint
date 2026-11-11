using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetUserProjectsLookupRequest
    : IQueryHandler<GetUserProjectsLookupRequest.Query, List<UserProjectLookupDto>>
{
    private readonly ICanTimeTrackUserBeRetrievedWithSelfBusinessRule _canTimeTrackUserBeRetrievedWithSelfBusinessRule;
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetUserProjectsLookupRequest(
        ICanTimeTrackUserBeRetrievedWithSelfBusinessRule canTimeTrackUserBeRetrievedWithSelfBusinessRule,
        ITimeTrackDbContext timeTrackDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _canTimeTrackUserBeRetrievedWithSelfBusinessRule = canTimeTrackUserBeRetrievedWithSelfBusinessRule;
        _timeTrackDbContext = timeTrackDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<List<UserProjectLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var canBeRetrievedResult = await _canTimeTrackUserBeRetrievedWithSelfBusinessRule.IsCorrespondsToBusiness(
            new CanTimeTrackUserBeRetrievedWithSelfBusinessRuleModel(request.UserId),
            cancellationToken);

        if (!canBeRetrievedResult.IsSuccess)
        {
            throw new ActionNotAllowedException();
        }

        return await GetProjectsAsync(request.UserId, cancellationToken);
    }

    private async Task<List<UserProjectLookupDto>> GetProjectsAsync(int userId, CancellationToken cancellationToken)
    {
        var displayProjectBillabilityAndClientName = CanUserViewProjectBillabilityAndClientName();

        var projects = await _timeTrackDbContext.ProjectUsers
            .AsNoTracking()
            .Include(projectUser => projectUser.Project)
                .ThenInclude(project => project!.Client)
            .Where(pu => pu.UserId == userId)
            .OrderByDescending(projectUser => projectUser.UserOnProjectFrom)
            .Select(projectUser => new UserProjectLookupDto
            {
                Id = projectUser.Id,
                ProjectId = projectUser.Project!.Id,
                ProjectName = projectUser.Project.Name,
                IsTimeOnProjectBillable = displayProjectBillabilityAndClientName ? projectUser.IsTimeOnProjectBillable : null,
                ClientName = displayProjectBillabilityAndClientName ? projectUser.Project.Client!.Name : null,
                UserOnProjectFrom = projectUser.UserOnProjectFrom,
                UserOnProjectTo = projectUser.UserOnProjectTo
            })
            .ToListAsync(cancellationToken);

        return projects;
    }

    private bool CanUserViewProjectBillabilityAndClientName()
    {
        if(_authenticatedUserContext.GetRoles().IsSystemAdministrator() ||
            _authenticatedUserContext.GetRoles().IsEmployee() ||
            _authenticatedUserContext.GetRoles().IsIntern())
        {
            return false;
        }

        return true;
    }

    internal sealed record Query(int UserId) : IQuery<List<UserProjectLookupDto>>;
}

