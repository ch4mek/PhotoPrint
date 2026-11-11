using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Payload.Core;

internal interface IGetPayloadUsersQueryBuilder
{
    IGetPayloadUsersQueryBuilder ForYear(int year);
    IGetPayloadUsersQueryBuilder WithFilter(PayloadFilterCoreDto filter);
    IGetPayloadUsersQueryBuilder WithOrdering(PayloadOrderingCoreDto ordering);
    IGetPayloadUsersQueryBuilder WithCareerStartDate();
    Task<int[]> BuildUserIds(CancellationToken token);
}

internal sealed class GetPayloadUsersQueryBuilder : IGetPayloadUsersQueryBuilder
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityContract _identityContract;

    private int _year;
    private PayloadFilterCoreDto? _filter;
    private PayloadOrderingCoreDto? _ordering;
    private bool _checkCareerStartDate;

    public GetPayloadUsersQueryBuilder(
        ITeamBoardDbContext teamBoardDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IIdentityContract identityContract)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _identityContract = identityContract;
    }

    public IGetPayloadUsersQueryBuilder ForYear(int year)
    {
        _year = year;
        return this;
    }

    public IGetPayloadUsersQueryBuilder WithFilter(PayloadFilterCoreDto filter)
    {
        _filter = filter;
        return this;
    }

    public IGetPayloadUsersQueryBuilder WithOrdering(PayloadOrderingCoreDto ordering)
    {
        _ordering = ordering;
        return this;
    }

    public IGetPayloadUsersQueryBuilder WithCareerStartDate()
    {
        _checkCareerStartDate = true;
        return this;
    }

    public async Task<int[]> BuildUserIs(CancellationToken token)
    {
        var query = _teamBoardDbContext.TeamBoardUsers
            .Where(x => x.Position != PortalRoleNames.Intern);

        if (_filter?.UserId is not null)
        {
            var success = int.TryParse(_filter.UserId, out var userId);
            if (!success)
            {
                return await Task.FromResult(Array.Empty<int>());
            }

            query = query.Where(x => userId == x.UserId);
        }

        query = FilterByUserName(query);
        query = FilterByProjectName(query);

        query = query.Where(x => x.Contracts.Any(c =>
            c.ContractFrom.Year <= _year
            && (!c.ContractTo.HasValue || c.ContractTo.Value.Year >= _year)
            && (!c.FireDate.HasValue || c.FireDate.Value.Year >= _year)));

        query = await FilterByRoles(query, token);

        query = FilterByNeedProject(query);

        query = FilterByComment(query);

        query = FilterByPlan(query);

        if (!(_filter?.SpecializationName).IsNullOrEmpty())
        {
            query = query.Where(x => x.UserSpecializations.Any(sp => sp.SpecializationTemplate!.Name == _filter.SpecializationName.Trim()));
        }

        query = await FilterByResourceGroup(query, token);

        var excludeRoles = new[] { PortalRoleNames.Ceo, PortalRoleNames.Cto };

        query = query.Where(x => !excludeRoles.Contains(x.Position));

        if (_checkCareerStartDate)
        {
            query = query.Where(x => x.CareerStartedAt != null);
        }

        var distinctUsers = query.Select(x => new
        {
            x.UserId,
            FullName = x.LastNameRu == null && x.FirstNameRu == null
                        ? x.LastName + " " + x.FirstName
                        : x.LastNameRu != null && x.FirstNameRu != null
                            ? x.LastNameRu + " " + x.FirstNameRu + " (" + x.LastName + " " + x.FirstName + ")"
                            : (x.LastNameRu ?? x.FirstNameRu) + " (" + x.LastName + " " + x.FirstName + ")"
        }).Distinct();

        if (_ordering?.Field == PayloadOrderingField.UserId)
        {
            distinctUsers = _ordering.IsDescending ?
                distinctUsers.OrderByDescending(x => x.UserId)
                : distinctUsers.OrderBy(x => x.UserId);
        }

        if (_ordering?.Field == PayloadOrderingField.EmployeeFullName)
        {
            distinctUsers = _ordering.IsDescending ?
                distinctUsers.OrderByDescending(x => x.FullName)
                : distinctUsers.OrderBy(x => x.FullName);
        }

        return await distinctUsers.Select(x => x.UserId).ToArrayAsync(token);
    }

    public async Task<int[]> BuildUserIds(CancellationToken token)
    {
        var query = _teamBoardDbContext.TeamBoardUsers
            .Where(x => x.Position != PortalRoleNames.Intern);

        var userId = ParseUserIdOrReturnEmpty(_filter?.UserId, out var shouldReturnEmpty);
        if (shouldReturnEmpty)
        {
            return Array.Empty<int>();
        }

        if (userId.HasValue)
        {
            query = query.Where(x => x.UserId == userId.Value);
        }

        query = await ApplyFiltersAsync(query, token);

        query = query
            .Where(NotExcludedRoles())
            .Where(HasCareerStartDateIfRequired());

        query = ApplyOrdering(query);

        return await query
            .Select(x => x.UserId)
            .ToArrayAsync(token);
    }

    private static int? ParseUserIdOrReturnEmpty(string? input, out bool shouldReturnEmpty)
    {
        shouldReturnEmpty = false;

        if (input is null)
        {
            return null;
        }

        if (!int.TryParse(input, out var userId))
        {
            shouldReturnEmpty = true;
            return null;
        }

        return userId;
    }

    private async Task<IQueryable<TeamBoardUser>> ApplyFiltersAsync(
        IQueryable<TeamBoardUser> query,
        CancellationToken token)
    {
        query = FilterByUserName(query);
        query = FilterByProjectName(query);
        query = await FilterByRoles(query, token);
        query = FilterByNeedProject(query);
        query = FilterByComment(query);
        query = FilterByPlan(query);

        query = query.Where(HasActiveContractInYear());

        if (!(_filter?.SpecializationName).IsNullOrEmpty())
        {
            var spec = _filter!.SpecializationName.Trim();
            query = query.Where(x =>
                x.UserSpecializations.Any(sp =>
                    sp.SpecializationTemplate!.Name == spec));
        }

        return await FilterByResourceGroup(query, token);
    }

    private Expression<Func<TeamBoardUser, bool>> HasActiveContractInYear()
    {
        return x => x.Contracts.Any(c =>
            c.ContractFrom.Year <= _year &&
            (!c.ContractTo.HasValue || c.ContractTo.Value.Year >= _year) &&
            (!c.FireDate.HasValue || c.FireDate.Value.Year >= _year));
    }

    private static Expression<Func<TeamBoardUser, bool>> NotExcludedRoles()
    {
        var excluded = new[] { PortalRoleNames.Ceo, PortalRoleNames.Cto };
        return x => !excluded.Contains(x.Position);
    }

    private Expression<Func<TeamBoardUser, bool>> HasCareerStartDateIfRequired()
    {
        return x => !_checkCareerStartDate || x.CareerStartedAt != null;
    }

    private IQueryable<TeamBoardUser> ApplyOrdering(IQueryable<TeamBoardUser> query)
    {
        if (_ordering == null)
        {
            return query;
        }

        Expression<Func<TeamBoardUser, string>> fullName = x =>
            string.IsNullOrEmpty(x.LastNameRu) && string.IsNullOrEmpty(x.FirstNameRu)
                ? (x.LastName + " " + x.FirstName).Trim()
                : !string.IsNullOrEmpty(x.LastNameRu) && !string.IsNullOrEmpty(x.FirstNameRu)
                    ? x.LastNameRu + " " + x.FirstNameRu + " (" + x.LastName + " " + x.FirstName + ")"
                    : x.LastNameRu ?? x.FirstNameRu + " (" + x.LastName + " " + x.FirstName + ")";

        return _ordering.Field switch
        {
            PayloadOrderingField.UserId =>
                _ordering.IsDescending
                    ? query.OrderByDescending(x => x.UserId)
                    : query.OrderBy(x => x.UserId),

            PayloadOrderingField.EmployeeFullName =>
                _ordering.IsDescending
                    ? query.OrderByDescending(fullName)
                    : query.OrderBy(fullName),

            _ => query
        };
    }

    private async Task<IQueryable<TeamBoardUser>> FilterByRoles(IQueryable<TeamBoardUser> query, CancellationToken token)
    {
        var roles = _authenticatedUserContext.GetRoles();

        if (!_authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
            !_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            if (_filter?.DepartmentIds == null || _filter.DepartmentIds.Count == 0)
            {
                return query;
            }

            var userIds = _filter.DepartmentIds.Count > 0
                ? await _identityContract.UsersWorkInDepartment(_year, _filter.DepartmentIds, token)
                : [];

            return query.Where(x => userIds.Contains(x.UserId));
        }

        var departmentId = _authenticatedUserContext.DepartmentId!;
        var isDepartmentInFilter = _filter?.DepartmentIds == null || _filter.DepartmentIds.Contains(departmentId);

        if (roles.IsHeadOfDepartment())
        {
            var userIds = isDepartmentInFilter
                ? await _identityContract.UsersWorkInDepartment(_year, [departmentId], token)
                : [];

            query = query.Where(x => userIds.Contains(x.UserId));
        }

        if (roles.IsGroupManager())
        {
            query = isDepartmentInFilter
                ? query.Where(x => x.ResourceGroupId == _authenticatedUserContext.ResourceGroupId)
                : query.Where(_ => false);
        }

        return query;
    }

    private async Task<IQueryable<TeamBoardUser>> FilterByResourceGroup(IQueryable<TeamBoardUser> query, CancellationToken token)
    {
        if (!(_filter?.ResourceGroupName).IsNullOrEmpty())
        {
            var keyword = $"%{_filter.ResourceGroupName.Trim()}%";
            var groupIds = await _teamBoardDbContext.TeamBoardResourceGroups
                .Where(g => g.IsActive)
                .Where(g => EF.Functions.Like(g.ResourceGroupName, keyword))
                .Select(x => x.ResourceGroupId)
                .ToListAsync(token);
            query = query
                .Where(x => x.ResourceGroupId != null)
                .Where(x => groupIds.Contains((int)x.ResourceGroupId!));
        }

        return query;
    }

    private IQueryable<TeamBoardUser> FilterByPlan(IQueryable<TeamBoardUser> query)
    {
        if (!(_filter?.Plan).IsNullOrEmpty())
        {
            var keyword = $"%{_filter.Plan.Trim()}%";
            query = query.Where(x => x.UserPayloadDetails
                .Any(payload => payload.Plan != null && EF.Functions.Like(payload.Plan, keyword)));
        }

        return query;
    }

    private IQueryable<TeamBoardUser> FilterByComment(IQueryable<TeamBoardUser> query)
    {
        if (!(_filter?.Comment).IsNullOrEmpty())
        {
            var keyword = $"%{_filter.Comment.Trim()}%";
            query = query.Where(x => x.Comments != null && EF.Functions.Like(x.Comments, keyword));
        }

        return query;
    }

    private IQueryable<TeamBoardUser> FilterByNeedProject(IQueryable<TeamBoardUser> query)
    {
        if (_filter?.NeedProject != null)
        {
            query = _filter.NeedProject.Value
                ? query.Where(x => x.UserPayloadDetails
                    .Any(payload => payload.NeedProject == _filter.NeedProject))
                : query.Where(x =>
                    x.UserPayloadDetails.Count == 0 ||
                    x.UserPayloadDetails.All(payload => payload.NeedProject == _filter.NeedProject));
        }

        return query;
    }

    private IQueryable<TeamBoardUser> FilterByProjectName(IQueryable<TeamBoardUser> query)
    {
        if (!(_filter?.ProjectName).IsNullOrEmpty())
        {
            var minDate = DateOnly.FromDateTime(DateTime.MinValue);
            var keyword = $"%{_filter.ProjectName.Trim()}%";
            query = query.Where(x => x.UserProjects.Any(up =>
                (up.UserOnProjectFrom == minDate || up.UserOnProjectFrom.Year <= _year) &&
                ((up.UserOnProjectTo == null || up.UserOnProjectTo == minDate)  || up.UserOnProjectTo.Value.Year >= _year) &&
                EF.Functions.Like(up.Project!.ProjectName, keyword)));
        }

        return query;
    }

    private IQueryable<TeamBoardUser> FilterByUserName(IQueryable<TeamBoardUser> query)
    {
        if (!(_filter?.UserFullName).IsNullOrEmpty())
        {
            var keyword = $"%{_filter.UserFullName.Trim()}%";
            query = query.Where(x => EF.Functions.Like(
                x.LastNameRu == null && x.FirstNameRu == null
                    ? x.LastName + " " + x.FirstName
                    : x.LastNameRu != null && x.FirstNameRu != null
                        ? x.LastNameRu + " " + x.FirstNameRu + " (" + x.LastName + " " + x.FirstName + ")"
                        : (x.LastNameRu ?? x.FirstNameRu) + " (" + x.LastName + " " + x.FirstName + ")",
                keyword));
        }

        return query;
    }
}

