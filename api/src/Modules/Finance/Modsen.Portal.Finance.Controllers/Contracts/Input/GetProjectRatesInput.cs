using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

namespace Modsen.Portal.Finance.Controllers.Contracts.Input;

public sealed class GetProjectRatesInput
{
    public int? UserId { get; set; }
    public string? UserFullName { get; set; }
    public IReadOnlyCollection<string>? DepartmentIds { get; set; }
    public string? ProjectName { get; set; }
    public string? ClientName { get; set; }
    public string? ClientLegalEntity { get; set; }

    public FinanceOrderingField? OrderingField { get; set; }
    public bool? IsDescendingOrdering { get; set; }
}
