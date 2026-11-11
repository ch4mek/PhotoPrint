using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Home.Core.Converters;
using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;

namespace Modsen.Portal.TeamBoard.Requests.Home.Requests;

internal sealed class GetBirthdaysByDateRequest : IQueryHandler<GetBirthdaysByDateRequest.Query, List<UserBirthdayInfoDto>>
{
    private readonly ITeamBoardDbContext _dbContext;
    private readonly ITeamboardUserToDepartmentNameConverter _dashboardUserToDepartmentNameConverter;

    public GetBirthdaysByDateRequest(
        ITeamBoardDbContext dbContext,
        ITeamboardUserToDepartmentNameConverter dashboardUserToDepartmentNameConverter)
    {
        _dbContext = dbContext;
        _dashboardUserToDepartmentNameConverter = dashboardUserToDepartmentNameConverter;
    }

    public async Task<List<UserBirthdayInfoDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        const int februaryNumber = 2;
        const int leapYearFebruaryLastDayNumber = 29;

        var todayDate = request.ForDate;
        var isCurrentYearLeap = DateTime.IsLeapYear(todayDate.Year);

        var birthdays = await _dbContext.TeamBoardUsers
            .Where(user => user.Position != PortalRoleNames.Intern)
            .Where(user => user.IsActive &&
                user.DateOfBirth.HasValue &&
                user.DateOfBirth.Value.Month == todayDate.Month &&
                user.DateOfBirth.Value.Day == todayDate.Day)
            .Include(user => user.Department)
            .Select(
                user => new UserBirthdayInfoDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FullName = $"{user.LastName} {user.FirstName}",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    AvatarFileId = user.AvatarFileId,
                    FirstNameRu = user.FirstNameRu,
                    LastNameRu = user.LastNameRu,
                    FullNameRu = (user.LastNameRu + ' ' + user.FirstNameRu).Trim(),
                    Department = _dashboardUserToDepartmentNameConverter.GetDepartmentName(user)
                })
            .ToListAsync(cancellationToken);

        if (!isCurrentYearLeap && todayDate is { Day: 28, Month: februaryNumber })
        {
            var leapYearBirthdays = await _dbContext.TeamBoardUsers
                .Where(user => user.IsActive &&
                    user.DateOfBirth.HasValue &&
                    user.DateOfBirth.Value.Month == februaryNumber &&
                    user.DateOfBirth.Value.Day == leapYearFebruaryLastDayNumber)
                .Include(user => user.Department)
                .Select(
                    user => new UserBirthdayInfoDto
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        FullName = $"{user.LastName} {user.FirstName}",
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        AvatarFileId = user.AvatarFileId,
                        FirstNameRu = user.FirstNameRu,
                        LastNameRu = user.LastNameRu,
                        FullNameRu = (user.LastNameRu + ' ' + user.FirstNameRu).Trim(),
                        Department = _dashboardUserToDepartmentNameConverter.GetDepartmentName(user)
                    })
                .OrderBy(birthday => birthday.FullName)
                .ToListAsync(cancellationToken);

            birthdays.AddRange(leapYearBirthdays);
        }

        return birthdays.OrderBy(birthday => birthday.FullName).ToList();
    }

    internal sealed record Query(DateOnly ForDate) : IQuery<List<UserBirthdayInfoDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(query => query.ForDate).NotNull();
        }
    }
}
