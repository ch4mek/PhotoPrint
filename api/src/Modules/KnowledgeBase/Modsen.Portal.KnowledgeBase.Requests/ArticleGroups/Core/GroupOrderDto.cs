namespace Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Core;

internal sealed class GroupOrderDto
{
    public int Id { get; set; }

    public int OrderWeight { get; set; }

    public int? ParentGroupId { get; set; }

    public List<ArticleOrderDto> ArticleOrders { get; set; } = [];

    public List<GroupOrderDto> ChildGroups { get; set; } = [];
}
