using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

namespace Modsen.Portal.Finance.Controllers.Contracts.Output;

public sealed class ProjectRateShortOutput
{
    public int Id { get; init; }
    public decimal HourlyRate { get; init; }
    public required string CurrencyId { get; init; }
    public DateOnly RateFrom { get; init; }
    public bool CanRateBeManagedByCurrentUser { get; init; }
    public bool IsDeleted { get; set; }

    internal static ProjectRateShortOutput Of(ProjectRateShortCoreDto source)
    {
        return new ProjectRateShortOutput()
        {
            Id = source.Id,
            HourlyRate = source.HourlyRate,
            CurrencyId = source.CurrencyId,
            RateFrom = source.RateFrom,
            CanRateBeManagedByCurrentUser = source.CanRateBeManagedByCurrentUser,
            IsDeleted = source.IsDeleted
        };
    }
}
