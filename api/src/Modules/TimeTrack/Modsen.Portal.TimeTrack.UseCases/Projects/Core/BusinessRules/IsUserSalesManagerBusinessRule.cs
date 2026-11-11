using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;

internal class IsUserSalesManagerBusinessRule(
    ITimeTrackPortalSettingsAccessor portalSettingsAccessor,
    ITimeTrackDbContext timeTrackDbContext)
    : IIsUserSalesManagerBusinessRule
{
    public async Task IsCorrespondsToBusiness(int? salesManagerId, CancellationToken cancellationToken)
    {
        if (salesManagerId is null)
        {
            return;
        }

        var portalSettings = await portalSettingsAccessor.GetSettings(cancellationToken);

        var salesDepartmentId = portalSettings.SalesDepartmentId;

        var isSalesManagerExist =
            await timeTrackDbContext.TimeTrackUsers
                .AnyAsync(u => u.DepartmentId == salesDepartmentId && u.UserId == salesManagerId,
                    cancellationToken);

        if (!isSalesManagerExist)
        {
            throw new EntityConflictException("User with SalesManagerId not found or not from sales department");
        }
    }
}

internal interface IIsUserSalesManagerBusinessRule
{
    Task IsCorrespondsToBusiness(int? salesManagerId, CancellationToken cancellationToken);
}
