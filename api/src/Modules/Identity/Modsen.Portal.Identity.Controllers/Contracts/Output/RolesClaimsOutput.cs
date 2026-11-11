using Modsen.Portal.Identity.Requests.Claims.Requests.RequestDto;

namespace Modsen.Portal.Identity.Controllers.Contracts.Output;

public class RolesClaimsOutput
{
    public int RoleId { get; set; }
    public required string RoleName { get; set; }
    public IReadOnlyCollection<ClaimOutput> Claims { get; set; } = [];

    public static RolesClaimsOutput Of(RoleClaimsDto roleClaimsDto)
    {
        ArgumentNullException.ThrowIfNull(roleClaimsDto);
        return new RolesClaimsOutput
        {
            RoleId = roleClaimsDto.RoleId,
            RoleName = roleClaimsDto.RoleName,
            Claims = roleClaimsDto.Claims.Select(ClaimOutput.Of).ToList()
        };
    }
}
