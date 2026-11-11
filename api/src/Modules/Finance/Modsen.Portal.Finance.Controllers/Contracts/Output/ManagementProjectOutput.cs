using Modsen.Portal.Finance.Requests.Managements.Requests.RequestDtos;

namespace Modsen.Portal.Finance.Controllers.Contracts.Output;

public sealed class ManagementProjectOutput
{
    public int ProjectId { get; init; }
    public required string ProjectName { get; init; }
    public string? ProjectColor { get; init; }
    public int ClientId { get; init; }
    public required string ClientName { get; init; }
    public string? ClientLegalEntity { get; init; }

    public decimal? BillableTrackedHours { get; init; }

    public decimal? Income { get; init; }
    public decimal? HourlyRate { get; init; }
    public CurrencyOutput? HourlyRateCurrency { get; init; }

    public decimal? IncomeByn { get; init; }

    internal static ManagementProjectOutput Of(ManagementProjectDto source)
    {
        return new ManagementProjectOutput
        {
            ProjectId = source.ProjectId,
            ProjectName = source.ProjectName,
            ProjectColor = source.ProjectColor,
            ClientId = source.ClientId,
            ClientName = source.ClientName,
            ClientLegalEntity = source.ClientLegalEntity,
            BillableTrackedHours = source.BillableTrackedHours != decimal.Zero ? source.BillableTrackedHours : null,
            Income = source.Income != decimal.Zero ? source.Income : null,
            HourlyRate = source.HourlyRate != decimal.Zero ? source.HourlyRate : null,
            HourlyRateCurrency = source.HourlyRateCurrency != null ? CurrencyOutput.Of(source.HourlyRateCurrency) : null,
            IncomeByn = source.IncomeByn != decimal.Zero ? source.IncomeByn : null
        };
    }
}
