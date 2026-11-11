using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Requests.Claims.Core.CoreDto;

namespace Modsen.Portal.Identity.Controllers.Contracts.Output;

public class ClaimOutput
{
    public required string Name { get; set; }
    public required string Value { get; set; }
    public AllowedValueType ValueType { get; set; }

    public static ClaimOutput Of(ClaimCoreDto claimDto)
    {
        ArgumentNullException.ThrowIfNull(claimDto);
        return new ClaimOutput
        {
            Name = claimDto.ClaimType,
            Value = claimDto.ClaimValue,
            ValueType = claimDto.AllowedValueType
        };
    }
}
