using Modsen.Portal.KnowledgeBase.Requests.Articles.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

internal record ArticleReferenceOutput
{
    public int Id { get; set; }
    public string? InternalFileId { get; set; }
    public string? ExternalUrl { get; set; }

    public static ArticleReferenceOutput Of(ArticleReferenceDto articleReferenceCoreDto)
    {
        return new ArticleReferenceOutput
        {
            Id = articleReferenceCoreDto.Id,
            InternalFileId = articleReferenceCoreDto.InternalFileId,
            ExternalUrl = articleReferenceCoreDto.ExternalUrl
        };
    }
}
