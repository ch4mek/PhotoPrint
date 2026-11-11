using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetSortedPersonalProjectLookupRequest
    : IQueryHandler<GetSortedPersonalProjectLookupRequest.Query, List<ProjectSortedLookupDto>>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetSortedPersonalProjectLookupRequest(
        ITimeTrackDbContext timeTrackDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public Task<List<ProjectSortedLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var userId = _authenticatedUserContext.GetRequiredUserId();

        var projectsQuery = _timeTrackDbContext.Projects
            .Include(project => project.ProjectUsers)
            .Where(project => project.IsActive)
            .Where(project => project.IsPublic || project.ProjectUsers.Any(user => user.UserId == userId));

        if (!request.KeyWord.IsNullOrWhiteSpace())
        {
            projectsQuery = projectsQuery.Where(project => EF.Functions.Like(project.Name, $"{request.KeyWord}%"));
        }

        return projectsQuery
            .Select(project => new ProjectSortedLookupDto
            {
                Id = project.Id,
                Name = project.Name,
                Color = project.Color,
                IsPublic = project.IsPublic,
                CreatedById = project.CreatedById,
                Type = project.IsPublic
                    ? PortalProjectTypeNames.Public
                    : PortalProjectTypeNames.Private,
                ClientName = project.Client == null ? "" : project.Client.Name ,
                ProjectUsers = new List<ProjectUserInfoDto>()
            })
            .OrderBy(project => project.Type)
            .ThenBy(project => project.Name)
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(string? KeyWord) : IQuery<List<ProjectSortedLookupDto>>, ITimeTrackRequest;
}
