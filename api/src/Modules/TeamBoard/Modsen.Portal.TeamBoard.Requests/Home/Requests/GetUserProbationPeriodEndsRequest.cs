using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;

namespace Modsen.Portal.TeamBoard.Requests.Home.Requests;

internal sealed class GetUserProbationPeriodEndsRequest : IQueryHandler<GetUserProbationPeriodEndsRequest.Query, List<UserProbationPeriodEndDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IClockService _clockService;

    public GetUserProbationPeriodEndsRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IClockService clockService)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _clockService = clockService;
    }

    public Task<List<UserProbationPeriodEndDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var date = request.Date ?? _clockService.TodayUtc();

        var probationPeriodEndDates = _teamBoardDbContext.TeamBoardUsers
            .Where(user => user.Position != PortalRoleNames.Intern)
            .Where(x => x.IsActive)
            .Where(x => x.HireDate.HasValue)
            .Where(x => x.HireDate!.Value.AddMonths(PortalSharedConstants.ProbationPeriodInMonth).AddDays(-1) == date)
            .Select(x => new UserProbationPeriodEndDto
            {
                UserId = x.UserId,
                Email = x.Email,
                FullName = x.LastName + " " + x.FirstName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                FirstNameRu = x.FirstNameRu,
                LastNameRu = x.LastNameRu,
                FullNameRu = (x.LastNameRu + ' ' + x.FirstNameRu).Trim(),
                DepartmentId = x.DepartmentId,
            })
            .ToListAsync(cancellationToken);

        return probationPeriodEndDates;
    }

    internal sealed record Query(DateOnly? Date) : IQuery<List<UserProbationPeriodEndDto>>;
}
