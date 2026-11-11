using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Home.Core.Converters;
using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;

namespace Modsen.Portal.TeamBoard.Requests.Home.Requests;

internal sealed class GetAllBirthdaysRequest : IQueryHandler<GetAllBirthdaysRequest.Query, List<MonthWithBirthdaysDto>>
{
    private readonly ITeamBoardDbContext _dbContext;
    private readonly ITeamboardUserToDepartmentNameConverter _dashboardUserToDepartmentNameConverter;
    private readonly IClockService _clockService;

    public GetAllBirthdaysRequest(
        ITeamBoardDbContext dbContext,
        ITeamboardUserToDepartmentNameConverter dashboardUserToDepartmentNameConverter,
        IClockService clockService)
    {
        _dbContext = dbContext;
        _dashboardUserToDepartmentNameConverter = dashboardUserToDepartmentNameConverter;
        _clockService = clockService;
    }

    public async Task<List<MonthWithBirthdaysDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var isCurrentYearLeap = DateTime.IsLeapYear(_clockService.UtcNow.Year);

        var users = await GetFilteredQuery(request)
            .Include(user => user.Department)
            .ToArrayAsync(cancellationToken);

        var birthdays = users
            .GroupBy(user => user.DateOfBirth!.Value.Month)
            .Select(monthGroup => new MonthWithBirthdaysDto
            {
                Month = monthGroup.Key,
                Days = monthGroup
                    .GroupBy(user => user.DateOfBirth!.Value.Day)
                    .Select(dayGroup => new DayWithBirthdaysDto
                    {
                        Day = dayGroup.Key,
                        Users = dayGroup
                            .Select(user => new UserBirthdayDto
                            {
                                LastName = user.LastName,
                                FirstName = user.FirstName,
                                AvatarFileId = user.AvatarFileId,
                                Department = _dashboardUserToDepartmentNameConverter.GetDepartmentName(user)
                            })
                            .ToList()
                    })
                    .OrderBy(day => day.Day)
                    .ToList()
            })
            .OrderBy(month => month.Month)
            .ToList();

        return isCurrentYearLeap
            ? birthdays
            : FixDatesForNotLeapYears(birthdays);
    }

    private IQueryable<TeamBoardUser> GetFilteredQuery(Query request)
    {
        var query = _dbContext.TeamBoardUsers
            .Where(user => user.Position != PortalRoleNames.Intern)
            .Where(user => user.IsActive && user.DateOfBirth.HasValue);

        if (request.Month is not null)
        {
            query = query.Where(user => user.DateOfBirth!.Value.Month == request.Month);
        }

        if (!request.Keyword.IsNullOrWhiteSpace())
        {
            var pattern = $"%{request.Keyword.Trim()}%";

            query = query.Where(user =>
                EF.Functions.ILike(user.FirstName + " " + user.LastName, pattern) ||
                EF.Functions.ILike(user.LastName + " " + user.FirstName, pattern)
            );
        }

        return query;
    }

    private static List<MonthWithBirthdaysDto> FixDatesForNotLeapYears(List<MonthWithBirthdaysDto> birthdays)
    {
        var februaryBirthdays = birthdays.FirstOrDefault(birthday => birthday.Month == 2);

        if (februaryBirthdays is null)
        {
            return birthdays;
        }

        var february29ThBirthdays = februaryBirthdays.Days.FirstOrDefault(day => day.Day == 29);

        if (february29ThBirthdays is null)
        {
            return birthdays;
        }

        februaryBirthdays.Days.Remove(february29ThBirthdays);

        var february28ThBirthdays = februaryBirthdays.Days.FirstOrDefault(day => day.Day == 28);

        if (february28ThBirthdays is not null)
        {
            february28ThBirthdays.Users.AddRange(february29ThBirthdays.Users);
        }
        else
        {
            february29ThBirthdays.Day = 28;
            februaryBirthdays.Days.Add(february29ThBirthdays);
        }

        return birthdays;
    }

    internal sealed record Query(string? Keyword, int? Month) : IQuery<List<MonthWithBirthdaysDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(query => query.Month)
                .InclusiveBetween(1, 12)
                .When(query => query.Month is not null);
        }
    }
}
