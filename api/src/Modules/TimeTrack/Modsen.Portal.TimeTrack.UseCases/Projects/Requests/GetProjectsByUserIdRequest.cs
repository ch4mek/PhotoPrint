using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;
internal sealed class GetProjectsByUserIdRequest : IQueryHandler<GetProjectsByUserIdRequest.Query, List<ProjectWithUsersDto>>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IClockService _clockService;

    public GetProjectsByUserIdRequest(
        ITimeTrackDbContext timeTrackDbContext,
        IClockService clockService)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _clockService = clockService;
    }

    public Task<List<ProjectWithUsersDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var dateNow = DateOnly.FromDateTime(_clockService.UtcNow);

        return _timeTrackDbContext.Projects
            .AsNoTracking()
            .Where(project => project.ProjectUsers.Any(projectUser => projectUser.UserId == request.UserId &&
                projectUser.UserOnProjectFrom <= dateNow && (!projectUser.UserOnProjectTo.HasValue
                    || dateNow <= projectUser.UserOnProjectTo)))
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
                }).ToList()
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(int UserId) : IQuery<List<ProjectWithUsersDto>>, ITimeTrackRequest;
}
