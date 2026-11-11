using Modsen.Portal.Identity.Requests.Claims.Core.CoreDto;

namespace Modsen.Portal.Identity.Requests.Claims.Requests.RequestDto
{
    public class DepartmentClaimsDto
    {
        public required string DepartmentId { get; set; }
        public IReadOnlyCollection<ClaimCoreDto> Claims { get; set; } = [];
    }
}
