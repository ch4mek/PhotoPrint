using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Booking.Requests.Booking.Core.BusinessRules;

internal interface IIsAllowedToBookBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int targetWorkplaceId, CancellationToken cancellationToken);
}

internal class IsAllowedToBookBusinessRule : IIsAllowedToBookBusinessRule {

    private readonly IBookingDbContext _bookingDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public IsAllowedToBookBusinessRule(IBookingDbContext bookingDbContext,  IAuthenticatedUserContext authenticatedUserContext)
    {
        _bookingDbContext = bookingDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(int targetWorkplaceId, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.GetRoles().IsCeo() || _authenticatedUserContext.GetRoles().IsCto())
        {
            return true;
        }

        var workspace = await _bookingDbContext.Workplaces
            .Include(x => x.Workspace)
            .Where(x => x.Id == targetWorkplaceId)
            .Select(x => x.Workspace)
            .FirstOrDefaultAsync(cancellationToken);
        return workspace == null ||
            !workspace.IsClaimRequired ||
            _authenticatedUserContext.HasClaimWithValue(BookingClaimTypeNames.BookWorkplaceInSpace, workspace.InternalCode);
    }
}
