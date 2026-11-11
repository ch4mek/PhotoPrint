using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetUsersHistoricalDepartmentQuery : IQueryHandler<GetUsersHistoricalDepartmentQuery.Query, IReadOnlyDictionary<int, string>>
{
    private readonly IIdentityDbContext _identityDbContext;

    public GetUsersHistoricalDepartmentQuery(
        IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public async Task<IReadOnlyDictionary<int, string>> Handle(Query request, CancellationToken cancellationToken)
    {
        var startDate = new DateOnly(request.Year, request.Month, 1);
        var endDate = new DateOnly(request.Year, request.Month, DateTime.DaysInMonth(request.Year, request.Month));

        return await _identityDbContext.UserDepartmentHistory
            .Where(h => request.UserIds.Contains(h.UserId))
            .Where(h => h.DateTo == null || h.DateFrom <= h.DateTo)
            .Where(h => h.DateFrom <= endDate && (h.DateTo == null || h.DateTo >= startDate))
            .GroupBy(h => h.UserId)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.OrderBy(h => h.DateFrom).First().DepartmentId,
                cancellationToken);
    }

    internal sealed record Query(int Year, int Month, IReadOnlyCollection<int> UserIds)
        : IQuery<IReadOnlyDictionary<int, string>>, IIdentityRequest;
}
