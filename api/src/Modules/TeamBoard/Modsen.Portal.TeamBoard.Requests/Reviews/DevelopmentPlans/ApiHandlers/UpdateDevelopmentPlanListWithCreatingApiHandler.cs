using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.ApiHandlers;

internal interface IUpdateDevelopmentPlanListWithCreatingApiHandler
{
    Task<bool> UpdateAsync(
        int userId,
        IReadOnlyCollection<DevelopmentPlanFullCoreDto> developmentPlans,
        CancellationToken cancellationToken);
}

internal sealed class UpdateDevelopmentPlanListWithCreatingApiHandler : IUpdateDevelopmentPlanListWithCreatingApiHandler
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public UpdateDevelopmentPlanListWithCreatingApiHandler(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<bool> UpdateAsync(int userId, IReadOnlyCollection<DevelopmentPlanFullCoreDto> developmentPlans, CancellationToken cancellationToken)
    {
        var userDevelopmentPlans = await _teamBoardDbContext.DevelopmentPlans
            .Where(developmentPlan => developmentPlan.UserId == userId)
            .ToListAsync(cancellationToken);

        var invalidDate = developmentPlans
            .Where(inputPlan => inputPlan.EndDate.HasValue && inputPlan.DeadlineDate.HasValue && developmentPlans
                .Any(dbPlan => dbPlan.Id == inputPlan.Id &&
                    (inputPlan.EndDate.Value <= dbPlan.StartDate || inputPlan.DeadlineDate <= dbPlan.StartDate)))
            .ToList();

        if (invalidDate.Count != 0)
        {
            throw new InvalidDataAppException("One or more EndDates/DeadlineDates are earlier than StartDates");
        }

        foreach (var developmentPlan in developmentPlans)
        {
            var userDevelopmentPlan = userDevelopmentPlans
                .FirstOrDefault(userDevelopmentPlan => userDevelopmentPlan.Id == developmentPlan.Id);

            if (userDevelopmentPlan is null)
            {
                _teamBoardDbContext.DevelopmentPlans.Add(new DevelopmentPlan
                {
                    StartDate = developmentPlan.StartDate,
                    DeadlineDate = developmentPlan.DeadlineDate,
                    EndDate = developmentPlan.EndDate,
                    Summary = developmentPlan.Summary ?? string.Empty,
                    Notes = developmentPlan.Notes,
                    UserId = developmentPlan.UserId,
                    Source = developmentPlan.Source ?? DevelopmentPlanSource.Unknown,
                    ConversationId = developmentPlan.ConversationId
                });
            }
            else
            {
                userDevelopmentPlan.DeadlineDate = developmentPlan.DeadlineDate;
                userDevelopmentPlan.Summary = developmentPlan.Summary ?? string.Empty;
                userDevelopmentPlan.Notes = developmentPlan.Notes;
            }
        }

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
