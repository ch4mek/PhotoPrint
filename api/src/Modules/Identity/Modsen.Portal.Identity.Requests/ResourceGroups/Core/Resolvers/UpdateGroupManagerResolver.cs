using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.ResourceGroups.Core.Resolvers;
internal interface IUpdateGroupManagerResolver
{
    Task<UpdateGroupManagerResolver.Response> ResolveAsync(UpdateGroupManagerResolver.Request request, CancellationToken cancellationToken);
}

internal sealed class UpdateGroupManagerResolver : IUpdateGroupManagerResolver
{
    private readonly IIdentityDbContext _dbContext;

    public UpdateGroupManagerResolver(IIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Response> ResolveAsync(Request request, CancellationToken cancellationToken)
    {
        var oldGm = await _dbContext.Users.Where(u => u.ResourceGroupId == request.Id &&
                u.UserRoles.First().Role!.Name == PortalRoleNames.GroupManager)
            .FirstOrDefaultAsync(cancellationToken);

        var newGm = await _dbContext.Users.Where(u => u.Id == request.NewGroupManagerId &&
                u.UserRoles.First().Role!.Name == PortalRoleNames.GroupManager)
            .FirstOrDefaultAsync(cancellationToken);

        if (newGm != null)
        {
            newGm.ResourceGroupId = request.Id;
        }

        if (oldGm != null)
        {
            oldGm.ResourceGroupId = null;
        }

        await _dbContext.SaveChanges(cancellationToken);

        return new Response(oldGm?.Id, newGm?.Id, request.Id);
        
    }

    internal sealed record Request(int Id, int? NewGroupManagerId);

    internal sealed record Response(int? OldGroupManagerId, int? NewGroupManagerId, int UpdatedGroupId);
}
