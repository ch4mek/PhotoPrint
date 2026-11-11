using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

namespace Modsen.Portal.Finance.Controllers.Contracts.Output;

public sealed class ProjectRateGridItemOutput
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

    internal static ProjectRateGridItemOutput Of(ProjectRateGridItem source)
    {
        return new ProjectRateGridItemOutput
        {
            ProjectRateId = source.ProjectRateId,
            ProjectId = source.ProjectId,
            ProjectName = source.ProjectName,
            ProjectColor = source.ProjectColor,
            ClientId = source.ClientId,
            ClientName = source.ClientName,
            ClientLegalEntity = source.ClientLegalEntity,
            HourlyRate = source.HourlyRate,
            CurrencyId = source.CurrencyId,
            CanRateBeManagedByCurrentUser = source.CanRateBeManagedByCurrentUser,
        };
    }
}
