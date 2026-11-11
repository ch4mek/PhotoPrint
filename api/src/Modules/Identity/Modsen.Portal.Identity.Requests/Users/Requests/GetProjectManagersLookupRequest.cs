using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetProjectManagersLookupRequest : IQueryHandler<GetProjectManagersLookupRequest.Query, List<ProjectManagerLookupDto>>
{
    private readonly IIdentityDbContext _identityDbContext;

    public GetProjectManagersLookupRequest(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public Task<List<ProjectManagerLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return _identityDbContext.Users
            .Where(user => user.IsActive && user.Department != null)
            .Where(user => user.Department!.Claims
                .Any(x => x.Department!.Id == user.Department.Id
                && x.ClaimType == IdentityClaimTypeNames.AvailableProjectManagerRole))
            .OrderBy(user => user.Id)
            .Select(user => new ProjectManagerLookupDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.UserRoles.First().Role!.Name!,
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<List<ProjectManagerLookupDto>>, IIdentityRequest;
}
