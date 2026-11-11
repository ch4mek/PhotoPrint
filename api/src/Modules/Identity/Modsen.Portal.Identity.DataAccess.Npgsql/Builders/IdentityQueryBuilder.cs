using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Builders;

internal interface IIdentityQueryBuilder
{
    IIdentityQueryBuilder WithRoles(IReadOnlyCollection<string> roles);

    IIdentityQueryBuilder WithDepartment(string? departmentId);

    IIdentityQueryBuilder WithKeyWord(string? keyWord);

    IIdentityQueryBuilder WithActiveStatus(bool? isActive);

    IIdentityQueryBuilder WithResourceGroup(int? resourceGroupId);

    IIdentityQueryBuilder WithUserIds(IReadOnlyCollection<int>? userIds);

    IIdentityQueryBuilder IncludeResourceGroup();

    IIdentityQueryBuilder WithExcludingSelf(IReadOnlyCollection<int>? excludedUsers);

    IQueryable<User> Build();
}

internal class IdentityQueryBuilder : IIdentityQueryBuilder
{
    private readonly IIdentityDbContext _identityDbContext;

    private IReadOnlyCollection<string> _roles = new List<string>();
    private string? _departmentId;
    private string? _keyWord;
    private bool? _isActive;
    private int? _resourceGroupId;
    private bool _includeResourceGroup;
    private IReadOnlyCollection<int>? _excludedUsers;

    private IReadOnlyCollection<int>? _userIds;

    public IdentityQueryBuilder(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public IIdentityQueryBuilder WithRoles(IReadOnlyCollection<string> roles)
    {
        _roles = roles;

        return this;
    }

    public IIdentityQueryBuilder WithDepartment(string? departmentId)
    {
        _departmentId = departmentId;

        return this;
    }

    public IIdentityQueryBuilder WithKeyWord(string? keyWord)
    {
        _keyWord = keyWord;

        return this;
    }

    public IIdentityQueryBuilder WithActiveStatus(bool? isActive)
    {
        _isActive = isActive;

        return this;
    }

    public IIdentityQueryBuilder WithResourceGroup(int? resourceGroupId)
    {
        _resourceGroupId = resourceGroupId;

        return this;
    }

    public IIdentityQueryBuilder WithUserIds(IReadOnlyCollection<int>? userIds)
    {
        _userIds = userIds;

        return this;
    }

    public IIdentityQueryBuilder IncludeResourceGroup()
    {
        _includeResourceGroup = true;

        return this;
    }

    public IIdentityQueryBuilder WithExcludingSelf(IReadOnlyCollection<int>? excludedUsers)
    {
        _excludedUsers = excludedUsers;

        return this;
    }

    public IQueryable<User> Build()
    {
        var query = _identityDbContext.Users.AsNoTracking();

        if (_roles.Count > 0)
        {
            query = query.Where(u =>
                u.UserRoles.Select(ur => ur.Role!.Name).Intersect(_roles).Any());
        }

        if (_departmentId is not null && _userIds is not null)
        {
            query = query.Where(u => u.DepartmentId == _departmentId || _userIds.Contains(u.Id));
        }
        else if (_departmentId is not null)
        {
            query = query.Where(u => u.DepartmentId == _departmentId);
        }
        else if (_userIds is not null)
        {
            query = query.Where(u => _userIds.Contains(u.Id));
        }
        else
        {
            if (_resourceGroupId.HasValue)
            {
                query = query.Where(u => u.ResourceGroupId == _resourceGroupId!.Value);
            }
        }

        if (!_keyWord.IsNullOrWhiteSpace())
        {
            var searchPattern = $"{_keyWord}%";

            query = query.Where(c => EF.Functions.ILike(c.FirstName, searchPattern)
                || EF.Functions.ILike(c.LastName, searchPattern));
        }

        if (_isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == _isActive);
        }

        if (_includeResourceGroup)
        {
            query = query.Include(u => u.ResourceGroup);
        }

        if (_excludedUsers != null && _excludedUsers.Count > 0)
        {
            query = query.Where(x => !_excludedUsers.Contains(x.Id));
        }    

        return query;
    }
}
