using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Users.Requests.RequestDto;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetUsersFromResourceGroupRequest : IQueryHandler<GetUsersFromResourceGroupRequest.Query, List<RecourceGroupUserDto>>
{
    private readonly IIdentityDbContext _identityDbContext;

    public GetUsersFromResourceGroupRequest(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public Task<List<RecourceGroupUserDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return _identityDbContext.Users
            .Where(u => u.ResourceGroupId == request.ResourceGroupId)
            .Select(u => new RecourceGroupUserDto(u.Id, u.ResourceGroupId!.Value))
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(int ResourceGroupId) : IQuery<List<RecourceGroupUserDto>>, IIdentityRequest;
}
