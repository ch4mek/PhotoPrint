using Modsen.Portal.Finance.Requests.Core.CoreDto;

namespace Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

internal record ProjectRateHistoryCoreDto
{
    public int UserId { get; init; }
    public required UserCoreDto User { get; init; }
    public int ProjectId { get; init; }
    public required ProjectCoreDto Project { get; init; }
    public required List<ProjectRateShortCoreDto> ProjectRates {  get; set; }

    public bool CanRateBeAddedByCurrentUser { get; init; }
}
