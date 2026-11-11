using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;

internal interface IIsUserManagerBusinessRule : IBusinessRule<int>;

internal class IsUserManagerBusinessRule : IIsUserManagerBusinessRule
{
    private readonly IIdentityDbContext _identityDbContext;

    public IsUserManagerBusinessRule(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken)
    {
        return _identityDbContext.UserRoles
            .Distinct()
            .Where(x => x.UserId == userId)
            .AnyAsync(x => x.User!.IsActive && x.Role!.IsCompanyManager, cancellationToken);
    }
}
