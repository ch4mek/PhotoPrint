using Modsen.Portal.Identity.Requests.Claims.Core.CoreDto;

namespace Modsen.Portal.Identity.Requests.Claims.Requests.RequestDto
{
    public class RoleClaimsDto
    {
        public int RoleId { get; set; }
        public required string RoleName { get; set; }
        public IReadOnlyCollection<ClaimCoreDto> Claims { get; set; } = [];
    }
}
