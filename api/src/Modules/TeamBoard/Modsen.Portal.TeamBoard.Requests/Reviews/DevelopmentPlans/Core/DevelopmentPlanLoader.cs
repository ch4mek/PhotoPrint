using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Core
{
    internal interface IDevelopmentPlanLoader
    {
        Task<DevelopmentPlanCoreDto?> LoadAsync(int developmentPlanId, CancellationToken cancellationToken);
    }

    internal class DevelopmentPlanLoader : IDevelopmentPlanLoader
    {
        private readonly ITeamBoardDbContext _teamBoardDbContext;
        public DevelopmentPlanLoader(ITeamBoardDbContext teamBoardDbContext)
        {
            _teamBoardDbContext = teamBoardDbContext;
        }

        public async Task<DevelopmentPlanCoreDto?> LoadAsync(int developmentPlanId, CancellationToken cancellationToken)
        {
            var developmemtPlan = await _teamBoardDbContext.DevelopmentPlans
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == developmentPlanId, cancellationToken);

            return developmemtPlan == null
                ? null
                : new DevelopmentPlanCoreDto(
                    developmemtPlan.Id,
                    developmemtPlan.StartDate,
                    developmemtPlan.DeadlineDate,
                    developmemtPlan.EndDate,
                    developmemtPlan.Summary,
                    developmemtPlan.Notes,
                    developmemtPlan.UserId,
                    developmemtPlan.User?.Position,
                    developmemtPlan.Status
                );
        }
    }
}
