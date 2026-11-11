namespace Modsen.Portal.KnowledgeBase.Requests.Articles.Requests.RequestDto;

internal record TagDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
