using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

namespace Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

internal sealed class KitCompoundLookupDto
{
    public required string Name { get; set; }
    public int AmountInKit { get; set; }

    public static KitCompoundLookupDto Of(KitCompoundCoreDto coreDto)
    {
        return new KitCompoundLookupDto { Name = coreDto.Name, AmountInKit = coreDto.AmountInKit };
    }
}
