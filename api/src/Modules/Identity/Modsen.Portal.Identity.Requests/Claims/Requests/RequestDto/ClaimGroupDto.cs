using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Identity.Requests.Claims.Requests.RequestDto
{
    public class ClaimGroupDto
    {
        public required string Name { get; set; }
        public ClaimsGroupStatus Status { get; set; }
    }
}
