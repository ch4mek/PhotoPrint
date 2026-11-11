using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.Requests.PermissionAssignments.Core.DbQueries;

internal interface IGetPermissionAssignmentByIdDbQuery
{
    Task<PermissionAssignment?> ExecuteAsync(int id, CancellationToken cancellationToken);
}

internal class GetPermissionAssignmentByIdDbQuery : IGetPermissionAssignmentByIdDbQuery
{
    private readonly IIdentityDbContext _identityDbContext;

    public GetPermissionAssignmentByIdDbQuery(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public Task<PermissionAssignment?> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        return _identityDbContext.PermissionAssignments
            .FirstOrDefaultAsync(pa => pa.Id == id, cancellationToken);
    }
}
