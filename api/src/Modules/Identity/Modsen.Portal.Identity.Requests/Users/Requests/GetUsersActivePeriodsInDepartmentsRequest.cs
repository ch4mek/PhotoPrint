using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Users.Requests
{
    internal sealed class GetUsersActivePeriodsInDepartmentsRequest
        : IQueryHandler<GetUsersActivePeriodsInDepartmentsRequest.Query, Dictionary<int, DateRangeDto[]>?>
    {
        private readonly IIdentityDbContext _identityDbContext;

        public GetUsersActivePeriodsInDepartmentsRequest(
            IIdentityDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }

        public async Task<Dictionary<int, DateRangeDto[]>?> Handle(Query request, CancellationToken cancellationToken)
        {
            var userWithDepartmentHistory = await _identityDbContext.Users
                .Where(x => request.UserIds.Contains(x.Id))
                .Include(x => x.UserDepartmentHistory)
                .Select(x => new
                {
                    x.Id,
                    x.UserDepartmentHistory,
                })
                .ToListAsync(cancellationToken);

            var userWithDateRanges =
                request.DepartmentIds == null || request.DepartmentIds.Count == 0
                    ? null
                    : userWithDepartmentHistory.ToDictionary(
                        user => user.Id,
                        user => user.UserDepartmentHistory
                            .Where(h => request.DepartmentIds.Contains(h.DepartmentId))
                            .Where(h => h.DateTo == null || h.DateFrom <= h.DateTo)
                            .Select(h => new DateRangeDto
                            {
                                Start = h.DateFrom,
                                End = h.DateTo ?? DateOnly.MaxValue
                            })
                            .ToArray()
                    );

            return userWithDateRanges;
        }

        internal sealed record Query(IReadOnlyCollection<int> UserIds, IReadOnlyCollection<string>? DepartmentIds)
            : IQuery<Dictionary<int, DateRangeDto[]>?>;
    }
}
