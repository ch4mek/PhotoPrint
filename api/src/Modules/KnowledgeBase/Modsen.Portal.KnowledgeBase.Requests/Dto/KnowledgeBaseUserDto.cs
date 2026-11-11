namespace Modsen.Portal.KnowledgeBase.Requests.Dto;

internal record KnowledgeBaseUserDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string Lastname { get; set; }
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }
}
