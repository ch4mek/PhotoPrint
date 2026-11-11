using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.ApiHandlers;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans;

internal interface IDevelopmentPlansApi
{
    Task<bool> UpdateDevelopmentPlanListWithCreating(
        int userId,
        IReadOnlyCollection<DevelopmentPlanFullCoreDto> developmentPlans,
        CancellationToken cancellationToken);
}

internal sealed class DevelopmentPlansApi : IDevelopmentPlansApi
{
    private readonly IUpdateDevelopmentPlanListWithCreatingApiHandler _updateDevelopmentPlanListWithCreatingApi;

    public DevelopmentPlansApi(IUpdateDevelopmentPlanListWithCreatingApiHandler updateDevelopmentPlanListWithCreatingApi)
    {
        _updateDevelopmentPlanListWithCreatingApi = updateDevelopmentPlanListWithCreatingApi;
    }

    public Task<bool> UpdateDevelopmentPlanListWithCreating(
        int userId,
        IReadOnlyCollection<DevelopmentPlanFullCoreDto> developmentPlans,
        CancellationToken cancellationToken)
    {
        return _updateDevelopmentPlanListWithCreatingApi.UpdateAsync(userId, developmentPlans, cancellationToken);
    }
}
