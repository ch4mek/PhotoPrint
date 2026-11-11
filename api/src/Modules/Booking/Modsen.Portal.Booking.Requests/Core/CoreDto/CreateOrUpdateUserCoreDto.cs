namespace Modsen.Portal.Booking.Requests.Core.CoreDto;

internal class CreateOrUpdateUserCoreDto
{
    public int UserId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? FirstNameRu { get; init; }
    public string? LastNameRu { get; init; }
    public required string Email { get; init; }
    public string? DepartmentId { get; init; }
    public int? ResourceGroupId { get; init; }
    public string? Position { get; init; }
    public int? InitiatorId { get; init; }
}
