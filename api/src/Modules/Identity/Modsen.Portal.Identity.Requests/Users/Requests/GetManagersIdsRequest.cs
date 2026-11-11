using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetManagersIdsRequest : IQueryHandler<GetManagersIdsRequest.Query, List<int>>
{
    private readonly IIdentityDbContext _identityDbContext;

    public GetManagersIdsRequest(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public Task<List<int>> Handle(Query request, CancellationToken cancellationToken)
    {
        return _identityDbContext.UserRoles
            .Distinct()
            .Where(x => x.Role!.IsCompanyManager && x.User!.IsActive)
            .Select(x => x.UserId)
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<List<int>>, IIdentityRequest;
}
