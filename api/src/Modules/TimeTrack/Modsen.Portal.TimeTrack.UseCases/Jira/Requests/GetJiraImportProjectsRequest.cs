using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Dtos;

namespace Modsen.Portal.TimeTrack.UseCases.Jira.Requests;

internal sealed class GetJiraImportProjectsRequest :
    IQueryHandler<GetJiraImportProjectsRequest.Query, IReadOnlyCollection<JiraImportProjectDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITimeTrackDbContext _dbContext;

    public GetJiraImportProjectsRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ITimeTrackDbContext dbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<JiraImportProjectDto>> Handle(
        Query request,
        CancellationToken cancellationToken)
    {
        var userId = _authenticatedUserContext.GetRequiredUserId();

        return await _dbContext.JiraSettings
            .AsNoTracking()
            .Where(x => x.IsActive && x.UserId == userId)
            .OrderBy(x => x.Project!.Name)
            .Select(x => new JiraImportProjectDto
            {
                JiraSettingId = x.Id,
                ProjectId = x.ProjectId,
                ProjectName = x.Project!.Name,
                JiraProjectTag = x.JiraProjectTag
            })
            .ToListAsync(cancellationToken);
    }

    public sealed record Query : IQuery<IReadOnlyCollection<JiraImportProjectDto>>;
}
