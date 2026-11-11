using Modsen.Portal.Identity.Requests.Claims.Requests.RequestDto;

namespace Modsen.Portal.Identity.Controllers.Contracts.Output;

public class DepartmentsClaimsOutput
{
    public required string DepartmentId { get; set; }
    public IReadOnlyCollection<ClaimOutput> Claims { get; set; } = [];

    public static DepartmentsClaimsOutput Of(DepartmentClaimsDto departmentDto)
    {
        ArgumentNullException.ThrowIfNull(departmentDto);
        return new DepartmentsClaimsOutput
        {
            DepartmentId = departmentDto.DepartmentId,
            Claims = departmentDto.Claims.Select(ClaimOutput.Of).ToList()
        };
    }
}
