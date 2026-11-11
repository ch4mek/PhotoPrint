namespace Modsen.Portal.Identity.Contract.Models;

public record CrossWriterUserDto
{
    public int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? FirstNameRu { get; init; }
    public string? LastNameRu { get; init; }
    public string? PatronymicRu { get; init; }
    public string? Email { get; init; }
    public string? DepartmentId { get; init; }
    public int? ResourceGroupId { get; init; }
    public required string Position { get; set; }
}
