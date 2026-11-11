using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Tags.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Tags.Requests;

internal sealed class UpdateTagRequest : ICommandHandler<UpdateTagRequest.Command, TagDto>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public UpdateTagRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<TagDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var entity = await _knowledgeBaseDbContext.Tags.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken) ??
            throw new EntityNotFoundException($"Tag by id {request.Id} is not found");

        entity.Name = request.Name;

        await _knowledgeBaseDbContext.SaveChangesAsync(cancellationToken);

        return new TagDto
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }

    internal sealed record Command(int Id, string Name) : ICommand<TagDto>, ITransactionalRequest;
}
