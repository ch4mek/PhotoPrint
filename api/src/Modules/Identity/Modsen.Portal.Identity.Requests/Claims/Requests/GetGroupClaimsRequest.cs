using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Claims.Core.CoreDto;

namespace Modsen.Portal.Identity.Requests.Claims.Requests
{
    internal sealed class GetGroupClaimsRequest : IQueryHandler<GetGroupClaimsRequest.Query, List<GroupClaimsCoreDto>>
    {
        private readonly IIdentityDbContext _identityDbContext;

        public GetGroupClaimsRequest(IIdentityDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }

        public async Task<List<GroupClaimsCoreDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _identityDbContext.ClaimGroups
                .Include(x => x.Claims)
                .Select(x => new GroupClaimsCoreDto
                {
                    Name = x.Name,
                    Claims = x.Claims.Select(f => new ClaimCoreDto
                    {
                        OrderWeight = f.OrderWeight,
                        AllowedValueType = f.AllowedValueType,
                        ClaimType = f.ClaimType,
                        ClaimValue = string.Empty
                    })
                    .OrderBy(f => f.OrderWeight)
                    .ToList()
                }).ToListAsync(cancellationToken);
        }

        internal sealed record Query : IQuery<List<GroupClaimsCoreDto>>, IIdentityRequest;
    }
}
