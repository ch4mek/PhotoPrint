namespace Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

internal sealed class KitCompoundCoreDto
{
    public int Id { get; set; }
    public bool IsClothing { get; set; }
    public required string Name { get; set; }
    public int AmountInKit { get; set; }
}
