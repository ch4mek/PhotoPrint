namespace Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

internal class ProjectRateGridItem
{
    public int? ProjectRateId { get; set; }
    public int ProjectId { get; set; }
    public required string ProjectName { get; set; }
    public string? ProjectColor { get; set; }
    public int ClientId { get; set; }
    public required string ClientName { get; set; }
    public string? ClientLegalEntity { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? CurrencyId { get; set; }
    public bool CanRateBeManagedByCurrentUser { get; set; }
}
