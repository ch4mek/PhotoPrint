using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.Requests.Core.Builders;
using Modsen.Portal.Finance.Requests.Core.CoreDto;

namespace Modsen.Portal.Finance.Requests.FinanceUsers.Requests;

internal class FinanceUsersLookupRequest : IQueryHandler<FinanceUsersLookupRequest.Query, List<UserCoreDto>>
{
    private readonly IFinanceClientsAndUsersQueryBuilder _managedClientsAndFinanceUsersQueryBuilder;
    private readonly IFinanceDbContext _dbContext;

    public FinanceUsersLookupRequest(IFinanceClientsAndUsersQueryBuilder managedClientsQueryBuilder, IFinanceDbContext dbContext)
    {
        _managedClientsAndFinanceUsersQueryBuilder = managedClientsQueryBuilder;
        _dbContext = dbContext;
    }

    public async Task<List<UserCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var financeProjectUsersQuery = await _managedClientsAndFinanceUsersQueryBuilder.BuildAvailabletoViewFinanceProjectUsersAsync(
                _dbContext.FinanceProjectUsers.AsQueryable(),
                request.Year, request.Month, cancellationToken);

        var managedUsers = await financeProjectUsersQuery
            .Select(x => x.User!)
            .Distinct()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        var result = managedUsers.Select(x => new UserCoreDto
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            FirstNameRu = x.FirstNameRu,
            LastNameRu = x.LastNameRu,
            PatronymicRu = x.PatronymicRu,
            DepartmentId = x.DepartmentId,
        }).ToList();

        return result is null
            ? []
            : result;
    }

    internal sealed record Query (int Year, int Month) : IQuery<List<UserCoreDto>>;
}
