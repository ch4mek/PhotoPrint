using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Claims.Core.CoreDto;

namespace Modsen.Portal.Identity.Requests.Claims.Core
{
    internal interface IUserGroupClaimsLoader
    {
        Task<List<GroupClaimsCoreDto>> Load(int userId, CancellationToken cancellationToken);
    }

    internal class UserGroupClaimsLoader : IUserGroupClaimsLoader
    {
        private readonly IIdentityDbContext _identityDbContext;

        public UserGroupClaimsLoader(IIdentityDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }

        public Task<List<GroupClaimsCoreDto>> Load(int userId, CancellationToken cancellationToken)
        {
            return _identityDbContext.UserClaims
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.Claim)
                    .ThenInclude(c => c!.Group)
                .GroupBy(uc => uc.Claim!.Group)
                .Select(g => new GroupClaimsCoreDto
                {
                    Name = g.Key!.Name,
                    Claims = g.Select(uc => new ClaimCoreDto
                    {
                        AllowedValueType = uc.Claim!.AllowedValueType,
                        ClaimType = uc.Claim.ClaimType,
                        ClaimValue = uc.ClaimValue!
                    }).ToList()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
