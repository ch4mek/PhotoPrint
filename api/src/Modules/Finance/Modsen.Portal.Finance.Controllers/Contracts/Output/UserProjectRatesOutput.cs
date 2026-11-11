using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

namespace Modsen.Portal.Finance.Controllers.Contracts.Output;

public sealed class UserProjectRatesOutput
{
    public required UserOutput User { get; init; }
    public bool CanUserInfoBeRetrievedByCurrentUser { get; init; }
    public required IReadOnlyList<ProjectRateGridItemOutput> ProjectRates { get; init; }
    public decimal? TotalRate { get; init; }
    public string? CurrencyId { get; init; }

    internal static UserProjectRatesOutput Of(UserProjectRatesCoreDto source)
    {
        return new UserProjectRatesOutput
        {
            User = UserOutput.Of(source.User),
            CanUserInfoBeRetrievedByCurrentUser = source.CanUserInfoBeRetrievedByCurrentUser,
            ProjectRates = source.ProjectRates.Select(x => ProjectRateGridItemOutput.Of(x)).ToList(),
            TotalRate = source.TotalRate,
            CurrencyId = source.CurrencyId
        };
    }
}
