using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

internal record GroupOutput
{
    public int Id { get; set; }
    public int? ParentGroupId { get; set; }
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
    public bool IsAvailableOnMainScreen { get; set; }
    public int OrderWeight { get; set; }
    public KnowledgeBaseUserOutput? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public KnowledgeBaseUserOutput? UpdatedByUser { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public IReadOnlyCollection<GroupOutput> ChildGroups { get; set; } = [];
    public IReadOnlyCollection<GroupArticleOutput> Articles { get; set; } = [];

    public static GroupOutput Of(GroupDto groupDto)
    {
        return new GroupOutput
        {
            Id = groupDto.Id,
            ParentGroupId = groupDto.ParentGroupId,
            Name = groupDto.Name,
            Icon = groupDto.Icon,
            IsActive = groupDto.IsActive,
            IsAvailableOnMainScreen = groupDto.IsAvailableOnMainScreen,
            OrderWeight = groupDto.OrderWeight,
            CreatedByUser = KnowledgeBaseUserOutput.Of(groupDto.CreatedByUser),
            CreatedAtUtc = groupDto.CreatedAtUtc,
            UpdatedByUser = KnowledgeBaseUserOutput.Of(groupDto.UpdatedByUser),
            UpdatedAtUtc = groupDto.UpdatedAtUtc,
            ChildGroups = groupDto.ChildGroups.Select(Of).ToList(),
            Articles = groupDto.Articles.Select(GroupArticleOutput.Of).ToList()
        };
    }
}
