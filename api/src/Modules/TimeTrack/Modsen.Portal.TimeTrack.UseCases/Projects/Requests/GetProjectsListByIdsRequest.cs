using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetProjectsListByIdsRequest : IQueryHandler<GetProjectsListByIdsRequest.Query, List<ProjectWithUsersDto>>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetProjectsListByIdsRequest(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public Task<List<ProjectWithUsersDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.Projects
            .AsNoTracking()
            .Include(project => project.ProjectUsers)
            .Where(project => request.Ids.Contains(project.Id))
            .Select(project => new ProjectWithUsersDto
            {
                Id = project.Id,
                Name = project.Name,
                Color = project.Color,
                ClientId = project.ClientId,
                IsBillable = project.IsBillable,
                IsActive = project.IsActive,
                IsPublic = project.IsPublic,
                ManagerId = project.ManagerUserId,
                ProjectUsers = project.ProjectUsers.Select(projectUser => new ProjectUserInfoDto
                {
                    Id = projectUser.UserId,
                    FirstName = projectUser.User!.FirstName,
                    LastName = projectUser.User.LastName,
                    DepartmentId = projectUser.User.DepartmentId
                }).ToList(),
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(IReadOnlyCollection<int> Ids) : IQuery<List<ProjectWithUsersDto>>;
}
