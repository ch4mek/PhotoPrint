using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.Requests.Users.Requests.Queries;

internal interface IGetGroupByIdDbQuery
{
    Task<ResourceGroup?> ExecuteAsync(int groupId, CancellationToken cancellationToken);
}

internal sealed class GetGroupByIdDbQuery : IGetGroupByIdDbQuery
{
    private readonly IIdentityDbContext _dbContext;
    public GetGroupByIdDbQuery(IIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ResourceGroup?> ExecuteAsync(int groupId, CancellationToken cancellationToken)
    {
        var group = await _dbContext.ResourceGroups
            .Where(group => group.Id == groupId && group.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        return group;
    }
}
