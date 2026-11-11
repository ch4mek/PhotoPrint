using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetUserChiefIdRequest : IQueryHandler<GetUserChiefIdRequest.Query, int?>
{
    private readonly IIdentityDbContext _identityDbContext;

    public GetUserChiefIdRequest(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public async Task<int?> Handle(Query request, CancellationToken cancellationToken)
    {
        var department = await _identityDbContext.Users
            .Where(x => x.Id == request.UserId)
            .Select(x => x.Department)
            .FirstOrDefaultAsync(cancellationToken);

        if (department == null)
        {
            return null;
        }

        var chiefId = await _identityDbContext.Users
            .Where(x => x.IsActive)
            .Where(x => x.UserRoles.Any(r => r.RoleId == department.DepartmentReportingToRoleId))
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return chiefId != 0 ? chiefId : null;
    }

    internal sealed record Query(int UserId) : IQuery<int?>, IIdentityRequest;
}

