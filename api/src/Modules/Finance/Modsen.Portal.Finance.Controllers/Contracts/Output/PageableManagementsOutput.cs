using Modsen.Portal.Core.Utils;
using Modsen.Portal.Finance.Requests.Managements.Requests.RequestDtos;

namespace Modsen.Portal.Finance.Controllers.Contracts.Output;

public sealed class PageableManagementsOutput : PageableListOfItems<ManagementUserOutput>
{
    public decimal? Profit { get; init; }
    public decimal? Expenses { get; init; }

    public decimal? IncomeBynAmount { get; init; }
    public decimal? SalaryAmount { get; init; }
    public int? AverageKpi { get; init; }
    public string? AverageKpiColor { get; init; }

    internal static PageableManagementsOutput Of(PageableManagementsDto source)
    {
        return new PageableManagementsOutput
        {
            Items = source.Items.Select(ManagementUserOutput.Of).ToList(),
            Page = source.Page,
            PageSize = source.PageSize,
            Total = source.Total,
            Expenses = source.Expenses,
            Profit = source.Profit,
            IncomeBynAmount = source.IncomeBynAmount,
            SalaryAmount = source.SalaryAmount,
            AverageKpi = source.AverageKpi,
            AverageKpiColor = source.AverageKpiColor,
        };
    }
}
