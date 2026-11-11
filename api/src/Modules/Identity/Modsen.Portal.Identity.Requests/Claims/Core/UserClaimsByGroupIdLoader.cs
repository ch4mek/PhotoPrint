using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Claims.Core
{
    internal interface IUserClaimsByGroupIdLoader
    {
        Task<List<ClaimDto>> LoadAsync(string groupId, int userId, CancellationToken cancellationToken);
    }
    internal class UserClaimsByGroupIdLoader : IUserClaimsByGroupIdLoader
    {
        private readonly IIdentityDbContext _identityDbContext;

        public UserClaimsByGroupIdLoader(IIdentityDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }

        public async Task<List<ClaimDto>> LoadAsync(string groupId, int userId, CancellationToken cancellationToken)
        {
            var userClaims = await _identityDbContext.UserClaims
                .Where(uc => uc.UserId == userId &&
                             uc.Claim!.GroupId == groupId)
                .Select(uc => new Claim(uc.Claim!.ClaimType, uc.ClaimValue!))
                .ToListAsync(cancellationToken);

            var roleIds = await _identityDbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);

            var roleClaims = await _identityDbContext.RoleClaims
                .Where(rc => roleIds.Contains(rc.RoleId) &&
                             rc.Claim!.GroupId == groupId)
                .Select(rc => new Claim(rc.Claim!.ClaimType, rc.ClaimValue!))
                .ToListAsync(cancellationToken);

            var departmentIds = await _identityDbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.DepartmentId)
                .ToListAsync(cancellationToken);

            var departmentClaims = await _identityDbContext.DepartmentClaims
                .Where(dc => departmentIds.Contains(dc.DepartmentId) &&
                             dc.Claim!.GroupId == groupId)
                .Select(dc => new Claim(dc.Claim!.ClaimType, dc.ClaimValue))
                .ToListAsync(cancellationToken);

            return userClaims
                .Concat(roleClaims)
                .Concat(departmentClaims)
                .DistinctBy(x => x.Type)
                .Select(x => new ClaimDto
                {
                    ClaimType = x.Type,
                    ClaimValue = x.Value,
                })
                .ToList();
        }
    }
}
