using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetRolesClassificatorRequest : IQueryHandler<GetRolesClassificatorRequest.Query, List<RoleClassificationDto>>
{
    private readonly IIdentityDbContext _identityDbContext;

    public GetRolesClassificatorRequest(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public async Task<List<RoleClassificationDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await _identityDbContext.Roles
            .AsNoTracking()
            .Select(x => new RoleClassificationDto(x.Name!, x.Name!, x.PublicName))
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<List<RoleClassificationDto>>, IIdentityRequest;
}
