using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Merch.DataAccess.Npgsql;

namespace Modsen.Portal.Merch.Requests.Core.DbQueries;

internal interface IUpdateIsActiveForUserDbQuery
{
    Task Set(int userId, bool isActive, CancellationToken cancellationToken);
}

internal class UpdateIsActiveForUserDbQuery : IUpdateIsActiveForUserDbQuery
{
    private readonly IMerchDbContext _dbContext;

    public UpdateIsActiveForUserDbQuery(IMerchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Set(int userId, bool isActive, CancellationToken cancellationToken)
    {
        var user = await _dbContext.MerchUsers
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);

        user.IsActive = isActive;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

