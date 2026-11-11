using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.Core.CoreDto;

namespace Modsen.Portal.Finance.Requests.Core.DbQueries;

internal interface ICreateOrUpdateUserDbQuery
{
    Task Execute(CreateOrUpdateUserCoreDto request, CancellationToken cancellationToken);
}

internal class CreateOrUpdateUserDbQuery : ICreateOrUpdateUserDbQuery
{
    private readonly IFinanceDbContext _dbContext;

    public CreateOrUpdateUserDbQuery(IFinanceDbContext teamBoardDbContext)
    {
        _dbContext = teamBoardDbContext;
    }

    public async Task Execute(CreateOrUpdateUserCoreDto request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.FinanceUsers
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (user == null)
        {
            user = new FinanceUserEntity
            {
                Id = request.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,                
                Email = request.Email
            };

            _dbContext.FinanceUsers.Add(user);
        }               

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.FirstNameRu = request.FirstNameRu;
        user.LastNameRu = request.LastNameRu;
        user.PatronymicRu = request.PatronymicRu;
        user.Email = request.Email;
        user.DepartmentId = request.DepartmentId;
        user.ResourceGroupId = request.ResourceGroupId;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
