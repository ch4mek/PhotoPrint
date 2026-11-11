namespace Modsen.Portal.Identity.Requests.Claims.Core.CoreDto
{
    public class GroupClaimsCoreDto
    {
        public required string Name { get; set; }
        public IReadOnlyCollection<ClaimCoreDto> Claims { get; set; } = [];
    }
}
