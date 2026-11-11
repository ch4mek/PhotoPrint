using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.Core.Calculators;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;

namespace Modsen.Portal.TeamBoard.Requests.Home.Requests;

internal sealed class GetUsersAnniversariesRequest :
    IQueryHandler<GetUsersAnniversariesRequest.Query, List<UserAnniversaryDateDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IClockService _clockService;
    private readonly IDateCalculator _dateCalculator;

    public GetUsersAnniversariesRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IClockService clockService,
        IDateCalculator dateCalculator)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _clockService = clockService;
        _dateCalculator = dateCalculator;
    }

    public async Task<List<UserAnniversaryDateDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        DateOnly fromDate;
        DateOnly toDate;

        if (request.MonthNumber.HasValue)
        {
            (fromDate, toDate) = GetMonthBounds(request.MonthNumber.Value);
        }
        else
        {
            var today = _clockService.TodayUtc();
            fromDate = today;
            toDate = fromDate;
        }

        var currentDate = fromDate;
        var anniversaryDates = new List<UserAnniversaryDateDto>();

        var userWithHireDates = await _teamBoardDbContext.TeamBoardUsers
            .Where(user => user.Position != PortalRoleNames.Intern)
            .Where(x => x.IsActive)
            .Where(x => x.HireDate.HasValue)
            .Select(x => new UserAnniversaryDateDto
            {
                UserId = x.UserId,
                Email = x.Email,
                FullName = x.LastName + " " + x.FirstName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                FirstNameRu = x.FirstNameRu,
                LastNameRu = x.LastNameRu,
                FullNameRu = (x.LastNameRu + ' ' + x.FirstNameRu).Trim(),
                AnniversaryDate = x.HireDate!.Value,
                DepartmentId = x.DepartmentId,
            })
            .OrderBy(x => x.AnniversaryDate)
            .ToListAsync(cancellationToken);

        while (currentDate <= toDate)
        {
            anniversaryDates.AddRange(userWithHireDates
                .Where(x => _dateCalculator.IsWholeYearsPassed(x.AnniversaryDate, currentDate))
                .Select(x => new UserAnniversaryDateDto
                {
                    UserId = x.UserId,
                    Email = x.Email,
                    FullName = x.FullName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    FirstNameRu = x.FirstNameRu,
                    LastNameRu = x.LastNameRu,
                    FullNameRu = x.FullName,
                    AnniversaryDate = currentDate,
                    DepartmentId = x.DepartmentId,
                }));

            currentDate = currentDate.AddDays(1);
        }

        return anniversaryDates;
    }

    internal sealed record Query(
        int? MonthNumber = null)
        : IQuery<List<UserAnniversaryDateDto>>;

    private (DateOnly FirstDay, DateOnly LastDay) GetMonthBounds(int month)
    {
        var today = _clockService.TodayUtc();
        var year = today.Year;

        if (month < 1 || month > 12)
        {
            throw new InvalidDataAppException("Month value is invalid");
        }

        if (month < today.Month)
        {
            year++;
        }

        var firstDay = new DateOnly(year, month, 1);
        var lastDay = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

        return (firstDay, lastDay);
    }
}
