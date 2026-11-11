using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;

internal interface ICanUserBeProjectManagerByDepartmentIdBusinessRule : IBusinessRule<int>;

internal class CanUserBeProjectManagerByDepartmentIdBusinessRule
    : ICanUserBeProjectManagerByDepartmentIdBusinessRule
{
    private readonly IIdentityDbContext _identityDbContext;

    public CanUserBeProjectManagerByDepartmentIdBusinessRule(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken)
    {
        var user = await _identityDbContext.Users
            .Include(x => x.Department)
            .FirstOrDefaultAsync(x => x.Id == userId && x.DepartmentId != null, cancellationToken);

        if (user?.DepartmentId == null)
        {
            return false;
        }

        return await _identityDbContext.DepartmentClaims
            .AnyAsync(x => x.DepartmentId == user.DepartmentId
                && x.ClaimType == IdentityClaimTypeNames.AvailableProjectManagerRole, cancellationToken);
    }
}
