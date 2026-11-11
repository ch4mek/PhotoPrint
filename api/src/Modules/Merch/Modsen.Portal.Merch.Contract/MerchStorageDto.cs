namespace Modsen.Portal.Merch.Contract;

public record MerchStorageDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
}
