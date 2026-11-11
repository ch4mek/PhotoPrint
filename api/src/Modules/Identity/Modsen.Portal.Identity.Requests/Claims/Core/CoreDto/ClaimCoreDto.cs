using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Identity.Requests.Claims.Core.CoreDto
{
    public class ClaimCoreDto
    {
        public required string ClaimType { get; set; }
        public required string ClaimValue { get; set; }
        public int OrderWeight { get; set; }
        public AllowedValueType AllowedValueType { get; set; }
    }
}
