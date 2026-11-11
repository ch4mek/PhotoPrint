using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Claims.Core.CoreDto;
using Modsen.Portal.Identity.Requests.Claims.Requests.RequestDto;

namespace Modsen.Portal.Identity.Requests.Claims.Requests
{
    internal sealed class GetDepartmentClaimsRequest : IQueryHandler<GetDepartmentClaimsRequest.Query, List<DepartmentClaimsDto>>
    {
        private readonly IIdentityDbContext _identityDbContext;

        public GetDepartmentClaimsRequest(IIdentityDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }

        public async Task<List<DepartmentClaimsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var allClaims = await _identityDbContext.DepartmentClaims
                .Select(c => new
                {
                    c.ClaimType,
                    c.Claim!.AllowedValueType
                })
                .Distinct()
                .ToListAsync(cancellationToken);

            var departmentClaims = await _identityDbContext.DepartmentClaims
                .AsNoTracking()
                .GroupBy(x => x.DepartmentId)
                .Select(x => new DepartmentClaimsDto
                {
                    DepartmentId = x.Key,
                    Claims = x.Select(f => new ClaimCoreDto
                    {
                        ClaimValue = f.ClaimValue,
                        ClaimType = f.ClaimType,
                        AllowedValueType = f.Claim!.AllowedValueType
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            var result = departmentClaims.Select(department => new DepartmentClaimsDto
            {
                DepartmentId = department.DepartmentId,
                Claims = allClaims.Select(claimType => new ClaimCoreDto
                {
                    ClaimType = claimType.ClaimType,
                    ClaimValue = department.Claims.FirstOrDefault(c => c.ClaimType == claimType.ClaimType)?.ClaimValue ?? string.Empty,
                    AllowedValueType = claimType.AllowedValueType
                }).ToList()
            }).ToList();

            return result;
        }

        internal sealed record Query : IQuery<List<DepartmentClaimsDto>>, IIdentityRequest;
    }
}
