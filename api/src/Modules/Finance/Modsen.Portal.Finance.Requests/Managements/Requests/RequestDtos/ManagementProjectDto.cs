using Modsen.Portal.Finance.Requests.Core.CoreDto;

namespace Modsen.Portal.Finance.Requests.Managements.Requests.RequestDtos;

internal class ManagementProjectDto
{
    public int UserId { get; init; }
    public int ProjectId { get; init; }
    public required string ProjectName { get; init; }
    public string? ProjectColor { get; init; }
    public int ClientId { get; init; }
    public required string ClientName { get; init; }
    public string? ClientLegalEntity { get; init; }

    public decimal? BillableTrackedHours { get; set; }

    public decimal? Income { get; set; }
    public decimal? HourlyRate { get; init; }
    public CurrencyCoreDto? HourlyRateCurrency { get; init; }

    public decimal? IncomeByn { get; set; }
    public required CurrencyCoreDto BynCurrency { get; init; }
}
