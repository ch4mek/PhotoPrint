using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.PermissionAssignments.Core.DbQueries;

internal interface IDeletePermissionAssignmentsDbQuery
{
    Task ExecuteAsync(int[] ids, CancellationToken cancellationToken);
}

internal sealed class DeletePermissionAssignmentsDbQuery : IDeletePermissionAssignmentsDbQuery
{
    private readonly IIdentityDbContext _identityDbContext;

    public DeletePermissionAssignmentsDbQuery(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public Task ExecuteAsync(int[] ids, CancellationToken cancellationToken)
    {
        return _identityDbContext.PermissionAssignments.Where(pa => ids.Contains(pa.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }
}
