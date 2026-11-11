namespace Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

public class ProjectRateCoreDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public int ProjectId { get; init; }
    public decimal HourlyRate { get; init; }
    public required string CurrencyId { get; init; }
    public DateOnly RateFrom { get; init; }

    public FinanceUserCoreDto? User { get; init; }
    public FinanceProjectCoreDto? Project { get; init; }
}
