using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.Core.Calculators;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Calendar.Core.Dto;

namespace Modsen.Portal.TeamBoard.Requests.Calendar.Requests;

internal sealed class GetUsersByAnniversariesIntervalRequest(
    ITeamBoardDbContext teamBoardDbContext,
    IDateCalculator dateCalculator) : IQueryHandler<GetUsersByAnniversariesIntervalRequest.Query, List<TeamBoardUserBirthdayDto>>
{
    public async Task<List<TeamBoardUserBirthdayDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var users = await teamBoardDbContext.TeamBoardUsers
            .Where(x => x.Position != PortalRoleNames.Intern)
            .Where(user => user.HireDate != null)
            .Where(user => request.From <= new DateOnly(request.From.Year, user.HireDate!.Value.Month, user.HireDate!.Value.Day))
            .Where(user => request.To >= new DateOnly(request.From.Year, user.HireDate!.Value.Month, user.HireDate!.Value.Day))
            .Select(user => new
            {
                HireDate = user.HireDate!.Value,
                User = new TeamBoardUserBirthdayDto
                {
                    Id = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    AvatarFileId = user.AvatarFileId,
                    WorkedFullYears = 0,
                }
            })
            .ToListAsync(cancellationToken);

        return users
            .Where(user => dateCalculator.GetWholeYears(user.HireDate, request.To) > 0)
            .Select(user =>
            {
                user.User.WorkedFullYears = dateCalculator.GetWholeYears(user.HireDate, request.To);

                return user.User;
            })
            .ToList();
    }

    internal sealed record Query(DateOnly From, DateOnly To) : IQuery<List<TeamBoardUserBirthdayDto>>;
}
