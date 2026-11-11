using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Finance.DataAccess.Npgsql;

namespace Modsen.Portal.Finance.Requests.Core.DbQueries;

internal interface IUpdateIsActiveForUserDbQuery
{
    Task Set(int userId, bool isActive, CancellationToken cancellationToken);
}

internal class UpdateIsActiveForUserDbQuery : IUpdateIsActiveForUserDbQuery
{
    private readonly IFinanceDbContext _dbContext;

    public UpdateIsActiveForUserDbQuery(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Set(int userId, bool isActive, CancellationToken cancellationToken)
    {
        var user = await _dbContext.FinanceUsers
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);
 
        user.IsActive = isActive;

        await _dbContext.SaveChangesAsync(cancellationToken);            
    }
}
