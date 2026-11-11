using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.Core.CoreDto;

namespace Modsen.Portal.Cv.Requests.Core.DbQueries;

internal interface ICreateOrUpdateUserDbQuery
{
    Task Execute(CreateOrUpdateUserCoreDto request, CancellationToken cancellationToken);
}

internal class CreateOrUpdateUserDbQuery : ICreateOrUpdateUserDbQuery
{
    private readonly ICvDbContext _dbContext;

    public CreateOrUpdateUserDbQuery(ICvDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Execute(CreateOrUpdateUserCoreDto request, CancellationToken cancellationToken)
    {
        if (request.Position.IsIntern())
        {
            return;
        }

        var user = await _dbContext.CvUsers
            .FirstOrDefaultAsync(user => user.UserId == request.UserId, cancellationToken);

        if (user is null)
        {
            user = new CvUserEntity
            {
                UserId = request.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                IsActive = true
            };

            _dbContext.CvUsers.Add(user);
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.FirstNameRu = request.FirstNameRu;
        user.LastNameRu = request.LastNameRu;
        user.Email = request.Email;
        user.DepartmentId = request.DepartmentId;
        user.ResourceGroupId = request.ResourceGroupId;
        user.Position = request.Position;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
