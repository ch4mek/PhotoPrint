
using Modsen.Portal.Identity.Requests.Claims.Core.CoreDto;

namespace Modsen.Portal.Identity.Controllers.Contracts.Output;

public class ClaimGroupOutput
{
    public required string Name { get; set; }
    public IReadOnlyCollection<ClaimOutput> Claims { get; set; } = [];

    public static ClaimGroupOutput Of(GroupClaimsCoreDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new ClaimGroupOutput
        {
            Name = dto.Name,
            Claims = dto.Claims.Select(ClaimOutput.Of).ToList(),
        };
    }
}
