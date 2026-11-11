using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.IdentityPortalSettings;

namespace Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;

public interface IIsApplicationRolesCorrectBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(object? model, CancellationToken cancellationToken);
}

internal class IsApplicationRolesCorrectBusinessRule : IBusinessRule<object?>, IIsApplicationRolesCorrectBusinessRule
{
    private readonly ILogger<IsApplicationRolesCorrectBusinessRule> _logger;
    private readonly IIdentityPortalSettingsAccessor _identityPortalSettingsAccessor;
    private readonly IIdentityDbContext _identityDbContext;

    public IsApplicationRolesCorrectBusinessRule(
        ILogger<IsApplicationRolesCorrectBusinessRule> logger,
        IIdentityPortalSettingsAccessor identityPortalSettingsAccessor,
        IIdentityDbContext identityDbContext)
    {
        _logger = logger;
        _identityPortalSettingsAccessor = identityPortalSettingsAccessor;
        _identityDbContext = identityDbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(object? model, CancellationToken cancellationToken)
    {
        var settings = await _identityPortalSettingsAccessor.GetSettings(cancellationToken);
        var usersWithCtoRoleCount = await GetUsersInRoleAsync(PortalRoleNames.Cto, cancellationToken);
        if (usersWithCtoRoleCount > settings.MaxCtoCount)
        {
            _logger.LogWarning("[Authentication] There is more than {Count} CTO in the system", settings.MaxCtoCount);
            return false;
        }

        var usersWithCeoRoleCount = await GetUsersInRoleAsync(PortalRoleNames.Ceo, cancellationToken);
        if (usersWithCeoRoleCount > settings.MaxCeoCount)
        {
            _logger.LogWarning("[Authentication] There is more than {Count} CEO in the system", settings.MaxCeoCount);
            return false;
        }

        var usersWithHeadOfDepartmentRole = await GetHeadsGroupAsync(cancellationToken);
        var group = usersWithHeadOfDepartmentRole.FirstOrDefault(group => group.UsersIds.Count > settings.MaxHeadOfSpecificDepartmentCount);
        if (group != null)
        {
            _logger.LogWarning("[Authentication] There is more than {Count} Head of Department ({DepartmentId}) in the same department", 
                settings.MaxHeadOfSpecificDepartmentCount, group.DepartmentId);
            return false;
        }

        return true;
    }

    private Task<int> GetUsersInRoleAsync(string roleName, CancellationToken token)
    {
        return _identityDbContext.Users
            .CountAsync(user =>
                user.IsActive && user.UserRoles.Any(role => role.Role!.Name == roleName),
                cancellationToken: token);
    }

    private Task<List<GetHeadsOfDepartmentsGroupDbQueryResponse>> GetHeadsGroupAsync(CancellationToken token)
    {
        return _identityDbContext.Users
            .Where(user => user.IsActive)
            .Where(user => user.DepartmentId != null)
            .Where(user => user.UserRoles.Any(role => role.Role!.Name == PortalRoleNames.HeadOfDepartment))
            .GroupBy(user => user.DepartmentId)
            .Select(group =>
                new GetHeadsOfDepartmentsGroupDbQueryResponse
                {
                    DepartmentId = group.Key!,
                    UsersIds = group.Select(user => user.Id).ToList()
                })
            .ToListAsync(token);
    }

    private sealed class GetHeadsOfDepartmentsGroupDbQueryResponse
    {
        public List<int> UsersIds { get; init; } = [];
        public required string DepartmentId { get; init; }
    }
}
