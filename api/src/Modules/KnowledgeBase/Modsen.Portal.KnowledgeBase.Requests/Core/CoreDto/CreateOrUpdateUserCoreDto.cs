namespace Modsen.Portal.KnowledgeBase.Requests.Core.CoreDto;

internal class CreateOrUpdateUserCoreDto
{
    public int UserId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public string? DepartmentId { get; init; }
    public string? Position { get; init; }
}
