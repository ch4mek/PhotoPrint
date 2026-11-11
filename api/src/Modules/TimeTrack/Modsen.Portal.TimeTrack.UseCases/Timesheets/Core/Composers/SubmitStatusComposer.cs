using Modsen.Portal.Core.Context;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Composers;

internal interface ISubmitStatusComposer
{
    Task<TimesheetSubmitStatus> ComposeAsync(SubmitStatusCoreDto coreDto, CancellationToken cancellationToken);
}

internal sealed class SubmitStatusComposer : ISubmitStatusComposer
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITimeTrackDbContext _dbContext;

    public SubmitStatusComposer(IAuthenticatedUserContext authenticatedUserContext, ITimeTrackDbContext dbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _dbContext = dbContext;
    }

    public async Task<TimesheetSubmitStatus> ComposeAsync(SubmitStatusCoreDto coreDto, CancellationToken cancellationToken)
    {
        var createdStatus = new TimesheetSubmitStatus
        {
            UserId = coreDto.UserId,
            Year = coreDto.Year,
            Month = coreDto.Month,
            Date = new DateOnly(coreDto.Year, coreDto.Month, 1),
            Status = coreDto.Status,
            FactHoursSum = coreDto.FactTrackedInMinutesSum,
            BillableHoursSum = coreDto.BillableTrackedInMinutesSum,
            Snapshot = coreDto.Snapshot,
            DecisionBy = _authenticatedUserContext.GetRequiredUserId()
        };

        _dbContext.TimesheetSubmitStatuses.Add(createdStatus);
        await _dbContext.SaveChanges(cancellationToken);

        return createdStatus;
    }
}
