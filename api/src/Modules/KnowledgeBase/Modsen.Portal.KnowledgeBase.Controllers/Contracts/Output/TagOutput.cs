using Modsen.Portal.KnowledgeBase.Requests.Tags.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

internal class TagOutput
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public static TagOutput Of(TagDto tagDto)
    {
        return new TagOutput
        {
            Id = tagDto.Id,
            Name = tagDto.Name
        };
    }
}
