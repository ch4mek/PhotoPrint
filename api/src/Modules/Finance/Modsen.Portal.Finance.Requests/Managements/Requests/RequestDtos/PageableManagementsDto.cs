using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.Finance.Requests.Managements.Requests.RequestDtos;

internal class PageableManagementsDto : PageableListOfItems<ManagementUserDto>
{
    public decimal? Profit { get; init; }
    public decimal? Expenses { get; init; }

    public decimal? IncomeBynAmount { get; init; }
    public decimal? SalaryAmount { get; init; }
    public int? AverageKpi { get; init; }
    public string? AverageKpiColor { get; init; }

    public int Year { get; init; }
    public int Month { get; init; }
}
