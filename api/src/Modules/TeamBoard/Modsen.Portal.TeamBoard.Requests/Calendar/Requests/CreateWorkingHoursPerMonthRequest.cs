using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Calendar.Core.Dto;

namespace Modsen.Portal.TeamBoard.Requests.Calendar.Requests;

internal sealed class CreateWorkingHoursPerMonthRequest : ICommandHandler<CreateWorkingHoursPerMonthRequest.Command, WorkingHoursPerMonthDto>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public CreateWorkingHoursPerMonthRequest(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<WorkingHoursPerMonthDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var isCountryExist = await _teamBoardDbContext.Countries
            .AnyAsync(country => country.Id == request.CountryId, cancellationToken);

        if (!isCountryExist)
        {
            throw new EntityNotFoundException($"There is no country with Id {request.CountryId}");
        }

        var isWorkingHoursExisting = await _teamBoardDbContext.WorkingHoursPerMonths
            .Where(workingHours => workingHours.Year == request.Year)
            .Where(workingHours => workingHours.MonthOfYear == request.MonthOfYear)
            .Where(workingHours => workingHours.CountryId == request.CountryId)
            .AnyAsync(cancellationToken);

        if (isWorkingHoursExisting)
        {
            throw new EntityConflictException($"Entry countryId={request.CountryId} and monthOfYear={request.MonthOfYear} of {request.Year} already exists");
        }

        var entity = new WorkingHoursPerMonth()
        {
            Year = request.Year,
            MonthOfYear = request.MonthOfYear,
            CountryId = request.CountryId,
            Hours = request.Hours
        };

        await _teamBoardDbContext.WorkingHoursPerMonths.AddAsync(entity, cancellationToken);

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return new WorkingHoursPerMonthDto
        {
            Id = entity.Id,
            Year = entity.Year,
            MonthOfYear = entity.MonthOfYear,
            CountryId = entity.CountryId,
            Hours = entity.Hours
        };
    }

    internal sealed record Command(int Year, int MonthOfYear, int CountryId, int Hours)
        : ICommand<WorkingHoursPerMonthDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Year).GreaterThanOrEqualTo(2020);
            RuleFor(x => x.MonthOfYear).InclusiveBetween(1, 12);
            RuleFor(x => x.Hours).GreaterThanOrEqualTo(0).LessThanOrEqualTo(248);
        }
    }
}
