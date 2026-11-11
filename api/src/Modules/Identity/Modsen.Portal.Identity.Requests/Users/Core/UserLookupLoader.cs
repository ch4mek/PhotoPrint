using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Identity.DataAccess.Npgsql.Builders;
using Modsen.Portal.Identity.Requests.Users.Core.CoreDto;
using Modsen.Portal.Identity.Requests.Users.Requests.RequestDto;

namespace Modsen.Portal.Identity.Requests.Users.Core;

internal interface IUserLookupLoader
{
    Task<List<UserLookupDto>> LoadAsync(UserLookupLoaderRequestDto request, CancellationToken cancellationToken);
}

internal class UserLookupLoader : IUserLookupLoader
{
    private readonly IIdentityQueryBuilder _identityQueryBuilder;

    public UserLookupLoader(IIdentityQueryBuilder identityQueryBuilder)
    {
        _identityQueryBuilder = identityQueryBuilder;
    }

    public async Task<List<UserLookupDto>> LoadAsync(UserLookupLoaderRequestDto request, CancellationToken cancellationToken)
    {
        var filteredQuery = _identityQueryBuilder
            .WithKeyWord(request.KeyWord)
            .WithRoles(request.Roles)
            .WithDepartment(request.DepartmentId)
            .WithResourceGroup(request.ResourceGroupId)
            .WithUserIds(request.UserIds)
            .IncludeResourceGroup()
            .WithActiveStatus(request.IsActive)
            .WithExcludingSelf(request.ExcludedUsers)
            .Build();

        var users = await filteredQuery
            .OrderBy(x => x.Id)
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .Select(x => new UserLookupDto
            {
                Id = x.Id,
                Color = x.Color,
                AvatarFileId = x.AvatarFileId,
                DepartmentId = x.DepartmentId,

                ResourceGroupId = x.ResourceGroupId,
                ResourceGroupName = x.ResourceGroup == null || !x.ResourceGroup.IsActive
                    ? null
                    : x.ResourceGroup.Name,

                IsActive = x.IsActive,

                LastName = x.LastName,
                FirstName = x.FirstName,
                FirstNameRu = x.FirstNameRu,
                LastNameRu = x.LastNameRu,

                PositionDescription = x.UserRoles
                    .Select(f => f.Role!.PublicName)
                    .First(),
            }).ToListAsync(cancellationToken);

        return users;
    }
}
