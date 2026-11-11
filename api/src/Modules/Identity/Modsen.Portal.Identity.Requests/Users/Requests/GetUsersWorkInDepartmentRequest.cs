using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Users.Requests
{
    internal sealed class GetUsersWorkInDepartmentRequest
        : IQueryHandler<GetUsersWorkInDepartmentRequest.Query, List<int>>
    {
        private readonly IIdentityDbContext _identityDbContext;

        public GetUsersWorkInDepartmentRequest(
            IIdentityDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }

        public async Task<List<int>> Handle(Query request, CancellationToken cancellationToken)
        {
            DateOnly startDate;
            DateOnly endDate;

            if (request.Year == default)
            {
                return await _identityDbContext.UserDepartmentHistory
                    .Where(h => request.DepartmentIds.Contains(h.DepartmentId))
                    .Select(h => h.UserId)
                    .Distinct()
                    .ToListAsync(cancellationToken);
            }

            if (request.Month is not null)
            {
                startDate = new DateOnly(request.Year, request.Month.Value, 1);
                endDate = new DateOnly(request.Year, request.Month.Value, DateTime.DaysInMonth(request.Year, request.Month.Value));
            }
            else
            {
                startDate = new DateOnly(request.Year, 1, 1);
                endDate = new DateOnly(request.Year, 12, 31);
            }

            return await _identityDbContext.UserDepartmentHistory
                .Where(h => h.DateTo == null || h.DateFrom <= h.DateTo)
                .Where(h =>
                    request.DepartmentIds.Contains(h.DepartmentId) &&
                    h.DateFrom <= endDate &&
                    (h.DateTo == null || h.DateTo >= startDate)
                )
                .Select(h => h.UserId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        internal sealed record Query(IReadOnlyCollection<string> DepartmentIds, int Year, int? Month) : IQuery<List<int>>, IIdentityRequest;
    }
}
