using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Core.DbQueries;

internal interface ICreateOrUpdateUserDbQuery
{
    Task Execute(CreateOrUpdateUserCoreDto request, CancellationToken cancellationToken);
}

internal class CreateOrUpdateUserDbQuery : ICreateOrUpdateUserDbQuery
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public CreateOrUpdateUserDbQuery(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task Execute(CreateOrUpdateUserCoreDto request, CancellationToken cancellationToken)
    {
        if (request.Position.IsIntern())
        {
            return;
        }

        var user = await _knowledgeBaseDbContext.KnowledgeBaseUsers
            .Where(x => x.UserId == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            user = new KnowledgeBaseUserEntity
            {
                UserId = request.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email
            };

            _knowledgeBaseDbContext.KnowledgeBaseUsers.Add(user);
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.DepartmentId = request.DepartmentId!;

        await _knowledgeBaseDbContext.SaveChangesAsync(cancellationToken);
    }
}
