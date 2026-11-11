using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.Core.Calculators;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Calendar.Core.Dto;

namespace Modsen.Portal.TeamBoard.Requests.Calendar.Requests;

internal sealed class GetUsersByAnniversariesDateRequest(
    ITeamBoardDbContext teamBoardDbContext,
    IDateCalculator dateCalculator) : IQueryHandler<GetUsersByAnniversariesDateRequest.Query, List<TeamBoardUserBirthdayDto>>
{
    public async Task<List<TeamBoardUserBirthdayDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var users = await teamBoardDbContext.TeamBoardUsers
            .Where(x => x.Position != PortalRoleNames.Intern)
            .Where(user => user.HireDate != null)
            .Where(user => user.HireDate!.Value.Month == request.Date.Month)
            .Where(user => user.HireDate!.Value.Year < request.Date.Year)
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
                    WorkedFullYears = 0
                }
            })
            .ToListAsync(cancellationToken);


        return users
            .Where(user => dateCalculator.IsWholeYearsPassed(user.HireDate, request.Date))
            .Select(user =>
            {
                user.User.WorkedFullYears = dateCalculator.GetWholeYears(user.HireDate, request.Date);

                return user.User;
            })
            .ToList();
    }

    internal sealed record Query(DateOnly Date) : IQuery<List<TeamBoardUserBirthdayDto>>;
}
