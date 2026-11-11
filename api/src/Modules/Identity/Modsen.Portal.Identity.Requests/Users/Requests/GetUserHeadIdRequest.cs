using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Core;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetUserHesdIdRequest : IQueryHandler<GetUserHesdIdRequest.Query, int?>
{
    private readonly IUserLoader _userLoader;
    private readonly IIdentityDbContext _identityDbContext;

    public GetUserHesdIdRequest(
        IIdentityDbContext identityDbContext,
        IUserLoader userLoader)
    {
        _identityDbContext = identityDbContext;
        _userLoader = userLoader;
    }

    public async Task<int?> Handle(Query request, CancellationToken cancellationToken)
    {
        var user = await _userLoader.LoadAsync(request.UserId, cancellationToken);

        if (user.Position == PortalRoleNames.Employee ||
            user.Position == PortalRoleNames.SystemAdministrator ||
            user.Position == PortalRoleNames.GroupManager)
        {
            var id =  await _identityDbContext.Users
                .Where(x => x.IsActive && x.UserRoles.Any(role => role.Role!.Name == PortalRoleNames.HeadOfDepartment))
                .Where(x => x.DepartmentId == user.DepartmentId)
                .Select(x => x.Id)
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
