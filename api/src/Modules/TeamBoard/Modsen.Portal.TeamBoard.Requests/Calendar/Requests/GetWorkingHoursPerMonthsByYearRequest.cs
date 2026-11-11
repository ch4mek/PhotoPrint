using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Calendar.Core.Dto;

namespace Modsen.Portal.TeamBoard.Requests.Calendar.Requests;

internal sealed class GetWorkingHoursPerMonthsByYearRequest
    : IQueryHandler<GetWorkingHoursPerMonthsByYearRequest.Query, WorkingHoursPerMonthsSheetDto>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetWorkingHoursPerMonthsByYearRequest(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }


    public async Task<WorkingHoursPerMonthsSheetDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var workingHoursPerMonth = await _teamBoardDbContext.WorkingHoursPerMonths.AsNoTracking()
            .OrderBy(x => x.CountryId)
            .ThenBy(x => x.MonthOfYear)
            .Where(x => x.Year == request.Year)
            .Select(x => new WorkingHoursPerMonthDto
            {
                Id = x.Id,
                Year = x.Year,
                MonthOfYear = x.MonthOfYear,
                CountryId = x.CountryId,
                Hours = x.Hours
            })
            .ToListAsync(cancellationToken);

        var yearTotalHours = workingHoursPerMonth.GroupBy(x => x.CountryId).OrderBy(x => x.Key)
            .Select(x => new WorkingHoursTotalDto
            {
                CountryId = x.Key,
                Hours = x.Sum(f => f.Hours),
                Year = x.First().Year
            }).ToList();

        return new WorkingHoursPerMonthsSheetDto
        {
            HoursPerMonth = workingHoursPerMonth,
            HoursPerYear = yearTotalHours
        };
    }

    internal sealed record Query(int Year) : IQuery<WorkingHoursPerMonthsSheetDto>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Year).GreaterThanOrEqualTo(2020);
        }
    }
}
