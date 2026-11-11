using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Core;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetUserGroupManagerIdRequest : IQueryHandler<GetUserGroupManagerIdRequest.Query, int?>
{
    private readonly IUserLoader _userLoader;
    private readonly IIdentityDbContext _identityDbContext;

    public GetUserGroupManagerIdRequest(
        IIdentityDbContext identityDbContext,
        IUserLoader userLoader)
    {
        _identityDbContext = identityDbContext;
        _userLoader = userLoader;
    }

    public async Task<int?> Handle(Query request, CancellationToken cancellationToken)
    {
        var user = await _userLoader.LoadAsync(request.UserId, cancellationToken);

        if (user.Position is PortalRoleNames.Employee or PortalRoleNames.SystemAdministrator && user.ResourceGroupId != null)
        {
            var id = await _identityDbContext.Users
                .Where(u => u.IsActive)
                .Where(u => u.UserRoles.Any(ur => ur.Role!.Name == PortalRoleNames.GroupManager))
                .Where(u => u.ResourceGroupId == user.ResourceGroupId.Value)
                .Select(u => u.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (id == 0)
            {
                return null;
            }

            return id;
        }

        return null;
    }

    internal sealed record Query(int UserId) : IQuery<int?>, IIdentityRequest;
}
