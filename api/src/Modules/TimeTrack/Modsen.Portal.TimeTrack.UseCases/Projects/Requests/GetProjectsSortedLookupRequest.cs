using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetProjectsSortedLookupRequest
    : IQueryHandler<GetProjectsSortedLookupRequest.Query, List<ProjectSortedLookupDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetProjectsSortedLookupRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ITimeTrackDbContext timeTrackDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _timeTrackDbContext = timeTrackDbContext;
    }

    public Task<List<ProjectSortedLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = _timeTrackDbContext.Projects
            .AsNoTracking();

        if (!request.KeyWord.IsNullOrWhiteSpace())
        {
            query = query.Where(project => EF.Functions.Like(project.Name, $"{request.KeyWord}%"));
        }

        if (_authenticatedUserContext.GetRoles().IsEmployee() || _authenticatedUserContext.GetRoles().IsSystemAdministrator())
        {
            query = query
                .Where(project => project.IsActive)
                .Where(project => project.IsPublic ||
                    project.ProjectUsers.Any(user => user.UserId == _authenticatedUserContext.UserId));
        }
        else if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            query = query
                .Where(project => project.IsActive)
                .Where(project => project.IsPublic ||
                    project.CreatedById == _authenticatedUserContext.GetRequiredUserId() ||
                    project.ProjectUsers.Any(user => user.User!.DepartmentId == _authenticatedUserContext.DepartmentId));
        }
        else if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            query = query
                .Where(project => project.IsActive)
                .Where(project => project.IsPublic ||
                    project.CreatedById == _authenticatedUserContext.GetRequiredUserId() ||
                    project.ProjectUsers.Any(user => user.User!.ResourceGroupId == _authenticatedUserContext.ResourceGroupId));
        }

        return query
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
                ClientName = project.Client!.Name,
                ProjectUsers = project.ProjectUsers.Select(projectUser => new ProjectUserInfoDto
                {
                    Id = projectUser.UserId,
                    FirstName = projectUser.User!.FirstName,
                    LastName = projectUser.User.LastName,
                    DepartmentId = projectUser.User.DepartmentId
                }).ToList(),

            })
            .OrderBy(project => project.Type)
            .ThenBy(project => project.Id)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(string? KeyWord)
        : IQuery<List<ProjectSortedLookupDto>>, ITimeTrackRequest;
}
