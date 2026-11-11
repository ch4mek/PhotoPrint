using Modsen.Portal.KnowledgeBase.Requests.Dto;

namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

internal record KnowledgeBaseUserOutput
{
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }

    public static KnowledgeBaseUserOutput? Of(KnowledgeBaseUserDto? knowledgeBaseUserDto)
    {
        if (knowledgeBaseUserDto == null)
        {
            return null;
        }

        return new KnowledgeBaseUserOutput
        {
            UserId = knowledgeBaseUserDto.Id,
            FirstName = knowledgeBaseUserDto.FirstName,
            LastName = knowledgeBaseUserDto.Lastname,
            Email = knowledgeBaseUserDto.Email,
            DepartmentId = knowledgeBaseUserDto.DepartmentId
        };
    }
}
