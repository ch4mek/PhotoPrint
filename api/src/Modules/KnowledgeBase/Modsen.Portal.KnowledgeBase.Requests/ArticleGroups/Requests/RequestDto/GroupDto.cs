using Modsen.Portal.KnowledgeBase.Requests.Dto;

namespace Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests.RequestDto;

internal class GroupDto
{
    public int Id { get; set; }
    public int? ParentGroupId { get; set; }
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
    public bool IsAvailableOnMainScreen { get; set; }
    public int OrderWeight { get; set; }
    public KnowledgeBaseUserDto? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public KnowledgeBaseUserDto? UpdatedByUser { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public IReadOnlyCollection<GroupDto> ChildGroups { get; set; } = [];
    public IReadOnlyCollection<GroupArticleDto> Articles { get; set; } = [];
};
    
