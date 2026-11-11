using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.Core.Calculators;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Calendar.Core.Dto;

namespace Modsen.Portal.TeamBoard.Requests.Calendar.Requests;

internal sealed class GetUsersByBirthdayIntervalRequest(
    ITeamBoardDbContext teamBoardDbContext,
    IDateCalculator dateCalculator) : IQueryHandler<GetUsersByBirthdayIntervalRequest.Query, List<TeamBoardUserBirthdayDto>>
{
    public async Task<List<TeamBoardUserBirthdayDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var users =  await teamBoardDbContext.TeamBoardUsers
            .Where(user => user.Position != PortalRoleNames.Intern)
            .Where(user => user.HireDate != null)
            .Where(user => user.DateOfBirth != null)
            .Where(user => new DateOnly(request.To.Year, user.DateOfBirth!.Value.Month, user.DateOfBirth!.Value.Day) >= request.From)
            .Where(user => new DateOnly(request.To.Year, user.DateOfBirth!.Value.Month, user.DateOfBirth!.Value.Day) <= request.To)
            .Select(user => new {
                DateOfBirth = user.DateOfBirth!.Value,
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

        return users.Select(user =>
            {
                user.User.WorkedFullYears = dateCalculator
                        .GetWholeYears(user.HireDate, new DateOnly(request.To.Year, user.DateOfBirth.Month, user.DateOfBirth.Day));

                return user.User;
            })
            .ToList();
    }

    internal sealed record Query(DateOnly From, DateOnly To) : IQuery<List<TeamBoardUserBirthdayDto>>;
}
