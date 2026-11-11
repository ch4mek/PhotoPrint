using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Clients.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Clients.Requests;

internal sealed class GetClientProjectsRequest : IQueryHandler<GetClientProjectsRequest.Query, List<ClientProjectDto>>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetClientProjectsRequest(
        ITimeTrackDbContext timeTrackDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task<List<ClientProjectDto>> Handle(
        Query request,
        CancellationToken cancellationToken)
    {
        var authenticatedUserId = _authenticatedUserContext.GetRequiredUserId();

        return _authenticatedUserContext.GetRoles() switch
        {
            var roles when (roles.IsEmployee() || roles.IsHeadOfDepartment()) && request.OnlyManagedProjects.HasValue && request.OnlyManagedProjects.Value => await GetManagedProjectsByClientIdAsync(request.ClientId, authenticatedUserId, cancellationToken),
            var roles when roles.IsGroupManager() => await GetResourceGroupProjectsByClientIdAsync(
                                request.ClientId,
                                authenticatedUserId,
                                _authenticatedUserContext.ResourceGroupId,
                                cancellationToken),
            var roles when roles.IsEmployee() || roles.IsSystemAdministrator() => await GetUserProjectsByClientIdAsync(
                                request.ClientId,
                                authenticatedUserId,
                                cancellationToken),
            var roles when roles.IsHeadOfDepartment() => await GetDepartmentProjectsByClientIdAsync(
                                request.ClientId,
                                authenticatedUserId,
                                _authenticatedUserContext.DepartmentId!,
                                cancellationToken),
            _ => await GetProjectsByClientIdAsync(request.ClientId, cancellationToken),
        };
    }

    private Task<List<ClientProjectDto>> GetResourceGroupProjectsByClientIdAsync(
        int clientId,
        int userId,
        int? resourceGroupId,
        CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.Projects
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.ProjectUsers)
            .ThenInclude(x => x.User)
            .Where(x =>
                x.IsActive &&
                x.ClientId == clientId &&
                    (x.IsPublic ||
                     x.CreatedById == userId ||
                     x.ProjectUsers.Any(projectUser => projectUser.User!.ResourceGroupId == resourceGroupId)))
            .OrderBy(x => x.Id)
            .Select(x => new ClientProjectDto
            {
                Id = x.Id,
                Name = x.Name,
                Color = x.Color,
                IsActive = x.IsActive,
                FactTrackedTime = null,
                BillableTrackedTime = null,
            })
            .ToListAsync(cancellationToken);
    }

    private Task<List<ClientProjectDto>> GetManagedProjectsByClientIdAsync(
        int clientId,
        int managerUserId,
        CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.Projects
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.ProjectUsers)
            .ThenInclude(x => x.User)
            .Where(x =>
                x.IsActive &&
                x.ClientId == clientId &&
                x.ManagerUserId == managerUserId)
            .OrderBy(x => x.Id)
            .Select(x => new ClientProjectDto
            {
                Id = x.Id,
                Name = x.Name,
                Color = x.Color,
                IsActive = x.IsActive,
                FactTrackedTime = x.FactTrackedTime,
                BillableTrackedTime = x.BillableTrackedTime,
            })
            .ToListAsync(cancellationToken);
    }

    private Task<List<ClientProjectDto>> GetUserProjectsByClientIdAsync(
        int clientId,
        int userId,
        CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.Projects
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.ProjectUsers)
            .ThenInclude(x => x.User)
            .Where(x =>
                x.IsActive &&
                x.ClientId == clientId &&
                (x.IsPublic || x.ProjectUsers.Any(projectUser => projectUser.UserId == userId)))
            .OrderBy(x => x.Id)
            .Select(x => new ClientProjectDto
            {
                Id = x.Id,
                Name = x.Name,
                Color = x.Color,
                IsActive = x.IsActive,
                FactTrackedTime = x.FactTrackedTime,
                BillableTrackedTime = x.BillableTrackedTime,
            })
            .ToListAsync(cancellationToken);
    }

    private Task<List<ClientProjectDto>> GetDepartmentProjectsByClientIdAsync(
        int clientId,
        int userId,
        string departmentId,
        CancellationToken cancellationToken)
    { 
        return _timeTrackDbContext.Projects
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.ProjectUsers)
            .ThenInclude(x => x.User)
            .Where(x =>
                x.IsActive &&
                x.ClientId == clientId &&
                   (x.IsPublic ||
                    x.CreatedById == userId ||
                    x.ProjectUsers.Any(projectUser => projectUser.User!.DepartmentId == departmentId)))
            .OrderBy(x => x.Id)
            .Select(x => new ClientProjectDto
            {
                Id = x.Id,
                Name = x.Name,
                Color = x.Color,
                IsActive = x.IsActive,
                FactTrackedTime = null,
                BillableTrackedTime = null,
            })
            .ToListAsync(cancellationToken);
    }

    private Task<List<ClientProjectDto>> GetProjectsByClientIdAsync(
        int clientId,
        CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.Projects
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.ProjectUsers)
            .ThenInclude(x => x.User)
            .Where(x => x.IsActive && x.ClientId == clientId)
            .OrderBy(x => x.Id)
            .Select(project => new ClientProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Color = project.Color,
                IsActive = project.IsActive,
                FactTrackedTime = project.Timesheets.Sum(timesheet => timesheet.FactTrackedInMinutes),
                BillableTrackedTime = project.Timesheets.Sum(timesheet => timesheet.BillableTrackedInMinutes),
            }).ToListAsync(cancellationToken);
    }

    internal sealed record Query(int ClientId, bool? OnlyManagedProjects) : IQuery<List<ClientProjectDto>>, ITimeTrackRequest;
}
