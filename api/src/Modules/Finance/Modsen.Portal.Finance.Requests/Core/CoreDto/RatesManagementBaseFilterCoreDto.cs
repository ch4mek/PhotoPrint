namespace Modsen.Portal.Finance.Requests.Core.CoreDto;

internal abstract class RatesManagementBaseFilterCoreDto
{
    public int? UserId { get; set; }
    public string? UserFullName { get; set; }
    public IReadOnlyCollection<string>? DepartmentIds { get; set; }
    public string? ProjectName { get; set; }
    public string? ClientName { get; set; }
    public string? ClientLegalEntity { get; set; }
}

internal class RatesFilterCoreDto : RatesManagementBaseFilterCoreDto;
internal class ManagementsFilterCoreDto : RatesManagementBaseFilterCoreDto;
