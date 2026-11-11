namespace Modsen.Portal.Equipment.Requests.Equipment.Core.CoreDto;

internal record UserFilterCoreDto
{
    public int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? FirstNameRu { get; init; }
    public string? LastNameRu { get; init; }
}
