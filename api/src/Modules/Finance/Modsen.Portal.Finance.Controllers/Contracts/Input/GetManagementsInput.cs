using Modsen.Portal.Finance.Requests.Managements.Core.CoreDto;

namespace Modsen.Portal.Finance.Controllers.Contracts.Input;

public sealed class GetManagementsInput
{
    public int? UserId { get; init; }
    public string? UserFullName { get; init; }
    public IReadOnlyCollection<string>? DepartmentIds { get; init; }
    public string? ProjectName { get; init; }
    public string? ClientName { get; init; }
    public string? ClientLegalEntity { get; init; }

    public int? PageSize { get; init; }
    public int? PageNumber { get; init; }

    public ManagementsOrderingField? OrderingField { get; init; }
    public bool? IsDescendingOrdering { get; init; }

    public bool ShouldCalculateMainStatistics { get; init; }
    public bool ShouldCalculateColumnStatistics { get; init; }
}
