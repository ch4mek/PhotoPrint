using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Providers;

internal class TimeTrackUserProvider : ITimeTrackUserProvider
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    public TimeTrackUserProvider(ITimeTrackDbContext dbContext)
    {
        _timeTrackDbContext = dbContext;
    }

    public async Task<Domain.TimeTrackUser?> GetByIdAsync(int userId, CancellationToken cancellationToken)
    {
        var timeTrackUser = await _timeTrackDbContext.TimeTrackUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        return TimeTrackUser.ToDomain(timeTrackUser);
    }

    public async Task<List<Domain.TimeTrackUser>> GetLookupWithParametersAsync(
        FilterParams filterParams,
        string? keyWord,
        CancellationToken token)
    {
        filterParams.IsActive = true;
        var query = GetFilteredUsersQuery(filterParams);

        if (!keyWord.IsNullOrWhiteSpace())
        {
            var searchPattern = $"{keyWord}%";
            query = query.Where(c => EF.Functions.Like(c.FirstName, searchPattern) ||
                EF.Functions.Like(c.LastName, searchPattern));
        }

        var timeTrackUsers = await query
            .OrderBy(x => x.UserId)
            .ToArrayAsync(token);

        return timeTrackUsers.Select(x => TimeTrackUser.ToDomain(x)!).ToList();
    }

    public async Task<List<Domain.TimeTrackUser>> GetLookupWithParametersForReportsAsync(
        FilterParams filterParams,
        bool isClientWithPublicProject,
        bool isPublicProject,
        bool isAnyPublicProjectManagedByUser,
        string? keyWord,
        CancellationToken token)
    {
        var query = StartActiveUsersQuery();

        query = ApplyUserIdFilter(query, filterParams);
        query = ApplyDepartmentFilter(query, filterParams);
        query = await ApplyResourceGroupFilterAsync(query, filterParams, token);

        var skipFiltering = await DetermineSkipFilteringAsync(filterParams, token);

        query = ApplyClientFilter(query, filterParams, skipFiltering);
        query = ApplyOnlyManagedUsersClientFilter(query, filterParams, isClientWithPublicProject);
        query = ApplyProjectFilter(query, filterParams, isPublicProject);
        query = ApplyAssignmentDatesFilter(query, filterParams, skipFiltering);

        var normalizedIsAnyPublicProjectManagedByUser = NormalizeIsAnyPublicProjectManagedByUser(
            isAnyPublicProjectManagedByUser,
            filterParams);
        query = ApplyOnlyManagedUsersManagerFilter(
            query,
            filterParams,
            isClientWithPublicProject,
            isPublicProject,
            normalizedIsAnyPublicProjectManagedByUser);

        query = ApplyKeywordFilter(query, keyWord);

        return await ExecuteUsersLookupAsync(query, token);
    }

    private IQueryable<TimeTrackUser> StartActiveUsersQuery()
    {
        return _timeTrackDbContext.TimeTrackUsers
            .Where(user => user.IsActive)
            .AsNoTracking();
    }

    private static IQueryable<TimeTrackUser> ApplyUserIdFilter(IQueryable<TimeTrackUser> query, FilterParams filterParams)
    {
        return filterParams.UserId.HasValue
            ? query.Where(user => user.UserId == filterParams.UserId)
            : query;
    }

    private static IQueryable<TimeTrackUser> ApplyDepartmentFilter(IQueryable<TimeTrackUser> query, FilterParams filterParams)
    {
        return filterParams.DepartmentId is not null && filterParams.OnlyManagedUsers != true
            ? query.Where(user => user.DepartmentId == filterParams.DepartmentId)
            : query;
    }

    private async Task<IQueryable<TimeTrackUser>> ApplyResourceGroupFilterAsync(
        IQueryable<TimeTrackUser> query,
        FilterParams filterParams,
        CancellationToken token)
    {
        if (!filterParams.ResourceGroupId.HasValue)
        {
            return query;
        }

        var inactiveGroups = await _timeTrackDbContext.TimeTrackResourceGroups
            .Where(g => !g.IsActive)
            .Select(g => g.ResourceGroupId)
            .ToListAsync(token);

        return query.Where(user =>
            filterParams.ResourceGroupId == 0
                ? (user.ResourceGroupId == null || inactiveGroups.Any(x => x == user.ResourceGroupId))
                : user.ResourceGroupId == filterParams.ResourceGroupId);
    }

    private async Task<bool> DetermineSkipFilteringAsync(FilterParams filterParams, CancellationToken token)
    {
        if (filterParams.ProjectId != null)
        {
            return await _timeTrackDbContext.Projects
                .AnyAsync(p => p.Id == filterParams.ProjectId && p.IsPublic, token);
        }

        if (filterParams is { ClientId: not null, ProjectId: null })
        {
            return await _timeTrackDbContext.Projects
                .AnyAsync(p => p.ClientId == filterParams.ClientId && p.IsPublic, token);
        }

        return false;
    }

    private static IQueryable<TimeTrackUser> ApplyClientFilter(
        IQueryable<TimeTrackUser> query,
        FilterParams filterParams,
        bool skipFiltering)
    {
        return filterParams.ClientId.HasValue
            ? query.Where(user => skipFiltering || user.Projects.Any(proj => proj.Project!.ClientId == filterParams.ClientId))
            : query;
    }

    private static IQueryable<TimeTrackUser> ApplyOnlyManagedUsersClientFilter(
        IQueryable<TimeTrackUser> query,
        FilterParams filterParams,
        bool isClientWithPublicProject)
    {
        if (filterParams is not { ClientId: not null, OnlyManagedUsers: not null } || !filterParams.OnlyManagedUsers.Value)
        {
            return query;
        }

        return query.Where(user => isClientWithPublicProject ||
            user.Projects.Any(proj => proj.Project!.ClientId == filterParams.ClientId &&
                proj.Project.ManagerUserId == filterParams.ManagerId));
    }

    private static IQueryable<TimeTrackUser> ApplyProjectFilter(
        IQueryable<TimeTrackUser> query,
        FilterParams filterParams,
        bool isPublicProject)
    {
        return filterParams.ProjectId.HasValue
            ? query.Where(user => isPublicProject || user.Projects.Any(proj => proj.ProjectId == filterParams.ProjectId))
            : query;
    }

    private static IQueryable<TimeTrackUser> ApplyAssignmentDatesFilter(
        IQueryable<TimeTrackUser> query,
        FilterParams filterParams,
        bool skipFiltering)
    {
        if (skipFiltering
            || filterParams is not { ReportForDateFrom: not null, ReportForDateTo: not null }
            || (filterParams.ProjectId == null && filterParams.ClientId == null))
        {
            return query;
        }

        return query.Where(user => user.Projects.Any(projectUser =>
            (filterParams.ProjectId == null || projectUser.ProjectId == filterParams.ProjectId) &&
            (filterParams.ClientId == null || projectUser.Project!.ClientId == filterParams.ClientId) &&
            (projectUser.UserOnProjectTo == null || projectUser.UserOnProjectTo >= filterParams.ReportForDateFrom) &&
            (projectUser.UserOnProjectFrom <= filterParams.ReportForDateTo)
        ));
    }

    private static bool NormalizeIsAnyPublicProjectManagedByUser(bool isAnyPublicProjectManagedByUser, FilterParams filterParams)
    {
        return isAnyPublicProjectManagedByUser
            && !filterParams.ClientId.HasValue
            && !filterParams.ProjectId.HasValue;
    }

    private static IQueryable<TimeTrackUser> ApplyOnlyManagedUsersManagerFilter(
        IQueryable<TimeTrackUser> query,
        FilterParams filterParams,
        bool isClientWithPublicProject,
        bool isPublicProject,
        bool isAnyPublicProjectManagedByUser)
    {
        if (filterParams is not { OnlyManagedUsers: true, ManagerId: not null }
            || isClientWithPublicProject
            || isPublicProject
            || isAnyPublicProjectManagedByUser)
        {
            return query;
        }

        return query.Where(user => user.Projects.Any(x => x.Project!.ManagerUserId == filterParams.ManagerId));
    }

    private static IQueryable<TimeTrackUser> ApplyKeywordFilter(IQueryable<TimeTrackUser> query, string? keyWord)
    {
        if (keyWord.IsNullOrWhiteSpace())
        {
            return query;
        }

        var searchPattern = $"{keyWord.ToLower(CultureInfo.InvariantCulture)}%";
        return query.Where(c => EF.Functions.Like(c.FirstName, searchPattern) ||
            EF.Functions.Like(c.LastName, searchPattern));
    }

    private static async Task<List<Domain.TimeTrackUser>> ExecuteUsersLookupAsync(
        IQueryable<TimeTrackUser> query,
        CancellationToken token)
    {
        var timeTrackUsers = await query
            .OrderBy(x => x.UserId)
            .ToListAsync(token);

        return timeTrackUsers.Select(x => TimeTrackUser.ToDomain(x)!).ToList();
    }

    private IQueryable<TimeTrackUser> GetFilteredUsersQuery(FilterParams filterParams)
    {
        var query = _timeTrackDbContext.TimeTrackUsers
            .AsNoTracking();

        query = filterParams.IsActive is not null ?
            query.Where(user => user.IsActive == filterParams.IsActive) :
            query;

        query = ApplyUsersFilter(query, filterParams);

        return query;
    }

    private static IQueryable<TimeTrackUser> ApplyUsersFilter(IQueryable<TimeTrackUser> query, FilterParams filterParams)
    {
        if (filterParams.UserId.HasValue)
        {
            query = query.Where(user => user.UserId == filterParams.UserId);
        }

        if (filterParams.DepartmentId is not null)
        {
            query = query.Where(user => user.DepartmentId == filterParams.DepartmentId);
        }

        if (filterParams.ClientId.HasValue)
        {
            query = query.Where(user => user.Projects.Any(proj => proj.Project!.ClientId == filterParams.ClientId));
        }

        if (filterParams.ProjectId.HasValue)
        {
            query = query.Where(user => user.Projects.Any(proj => proj.ProjectId == filterParams.ProjectId));
        }

        if (filterParams.ResourceGroupId.HasValue)
        {
            query = query.Where(user => user.ResourceGroupId == filterParams.ResourceGroupId);
        }

        return query;
    }
}
