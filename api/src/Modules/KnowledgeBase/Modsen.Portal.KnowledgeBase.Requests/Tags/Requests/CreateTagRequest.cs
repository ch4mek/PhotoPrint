using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Tags.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Tags.Requests;

internal sealed class CreateTagRequest : ICommandHandler<CreateTagRequest.Command, TagDto>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public CreateTagRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<TagDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var existingTag = await _knowledgeBaseDbContext.Tags
                .FirstOrDefaultAsync(x => x.Name == request.Name, cancellationToken);

        if (existingTag != null)
        {

            return new TagDto
            {
                Id = existingTag.Id,
                Name = existingTag.Name
            };
        }

        var tag = new TagEntity
        {
            Name = request.Name
        };

        _knowledgeBaseDbContext.Tags.Add(tag);

        await _knowledgeBaseDbContext.SaveChangesAsync(cancellationToken);

        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name
        };
    }

    internal sealed record Command(string Name) : ICommand<TagDto>, ITransactionalRequest;
}
