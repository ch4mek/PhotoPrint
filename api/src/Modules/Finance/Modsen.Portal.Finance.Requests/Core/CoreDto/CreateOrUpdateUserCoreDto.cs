namespace Modsen.Portal.Finance.Requests.Core.CoreDto;

internal record CreateOrUpdateUserCoreDto
{
    public int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? FirstNameRu { get; init; }
    public string? LastNameRu { get; init; }
    public string? PatronymicRu { get; init; }
    public required string Email { get; init; }
    public string? DepartmentId { get; init; }
    public int? ResourceGroupId { get; init; }
}
