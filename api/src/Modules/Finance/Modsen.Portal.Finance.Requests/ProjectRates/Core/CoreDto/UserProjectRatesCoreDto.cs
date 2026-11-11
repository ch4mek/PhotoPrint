using Modsen.Portal.Finance.Requests.Core.CoreDto;

namespace Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

internal class UserProjectRatesCoreDto
{
    public required UserCoreDto User { get; set; }
    public bool CanUserInfoBeRetrievedByCurrentUser { get; set; }
    public required List<ProjectRateGridItem> ProjectRates { get; set; }
    public decimal? TotalRate { get; set; }
    public string? CurrencyId { get; set; }
}
