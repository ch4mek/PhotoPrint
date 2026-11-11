namespace Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

internal record ProjectRateShortCoreDto
{
    public int Id { get; init; }
    public decimal HourlyRate { get; init; }
    public required string CurrencyId { get; init; }
    public DateOnly RateFrom { get; init; }
    public bool CanRateBeManagedByCurrentUser { get; set; }
    public bool IsDeleted { get; set; }
}
