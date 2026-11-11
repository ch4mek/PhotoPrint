using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Claims.Core.CoreDto;
using Modsen.Portal.Identity.Requests.Claims.Requests.RequestDto;

namespace Modsen.Portal.Identity.Requests.Claims.Requests
{
    internal sealed class GetRoleClaimsRequest : IQueryHandler<GetRoleClaimsRequest.Query, List<RoleClaimsDto>>
    {
        private readonly IIdentityDbContext _identityDbContext;

        public GetRoleClaimsRequest(IIdentityDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }

        public async Task<List<RoleClaimsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var allClaims = await _identityDbContext.Claims
                .Select(c => new
                {
                    c.ClaimType,
                    c.AllowedValueType
                })
                .ToListAsync(cancellationToken);

            var roleClaims = await _identityDbContext.UserRoles
                .AsNoTracking()
                .Include(x => x.Role)
                    .ThenInclude(f => f!.Claims)
                .GroupBy(x => new { x.RoleId, x.Role!.Name })
                .Select(x => new RoleClaimsDto
                {
                    RoleId = x.Key.RoleId,
                    RoleName = x.Key.Name!,
                    Claims = x.SelectMany(f => f.Role!.Claims)
                        .Select(f => new ClaimCoreDto
                        {
                            ClaimType = f.ClaimType!,
                            ClaimValue = f.ClaimValue!,
                            AllowedValueType = f.Claim!.AllowedValueType
                        }).ToList()
                })
                .ToListAsync(cancellationToken);

            var result = roleClaims.Select(role => new RoleClaimsDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Claims = allClaims.Select(claimType => new ClaimCoreDto
                {
                    ClaimType = claimType.ClaimType!,
                    ClaimValue = role.Claims.FirstOrDefault(c => c.ClaimType == claimType.ClaimType)?.ClaimValue ?? string.Empty,
                    AllowedValueType = claimType.AllowedValueType
                }).ToList()
            }).ToList();

            return result;
        }

        internal sealed record Query : IQuery<List<RoleClaimsDto>>, IIdentityRequest;
    }
}
