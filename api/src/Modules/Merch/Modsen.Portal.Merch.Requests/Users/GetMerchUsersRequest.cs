using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Users.RequestDto;

namespace Modsen.Portal.Merch.Requests.Users;

internal sealed class GetMerchUsersRequest: IQueryHandler<GetMerchUsersRequest.Query, List<MerchUserLookupDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IMerchDbContext _merchDbContext;

    public GetMerchUsersRequest(IAuthenticatedUserContext authenticatedUserContext, IMerchDbContext merchDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _merchDbContext = merchDbContext;
    }


    public async Task<List<MerchUserLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (!_authenticatedUserContext.HasClaim(MerchClaimTypeNames.ManageMerchTransactions.Name))
        {
            return [];
        }

        return await _merchDbContext.MerchUsers
            .AsNoTracking()
            .Where(u => u.IsActive)
            .OrderBy(user => user.UserId)
            .Select(user => new MerchUserLookupDto
            {
                Id = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DepartmentId = user.DepartmentId,
                Email =  user.Email,
            })
            .ToListAsync(cancellationToken);

    }

    internal sealed record Query : IQuery<List<MerchUserLookupDto>>;
}
