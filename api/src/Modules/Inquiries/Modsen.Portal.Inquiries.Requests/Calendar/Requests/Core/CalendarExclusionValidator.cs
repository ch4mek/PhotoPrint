using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Inquiries.Requests.Calendar.Requests.Core;

internal interface ICalendarExclusionValidator
{
    Task Validate(CalendarExclusionEntity entity, CancellationToken cancellationToken);
}

internal class CalendarExclusionValidator : ICalendarExclusionValidator
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly ITeamBoardContract _teamBoardContract;

    public CalendarExclusionValidator(
        IInquiriesDbContext inquiriesDbContext,
        ITeamBoardContract teamBoardContract)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _teamBoardContract = teamBoardContract;
    }

    public async Task Validate(CalendarExclusionEntity entity, CancellationToken cancellationToken)
    {
        if (entity.NewDate == entity.Date)
        {
            throw new ValidationFailedException("New workday date coincides with date. Please check and try again");
        }

        var countries = await _teamBoardContract.GetCountries(cancellationToken);

        if (countries.All(country => country.Id != entity.CountryId))
        {
            throw new EntityNotFoundException("Country is not exist");
        }

        var isDateExist = await _inquiriesDbContext.CalendarExclusions
            .Where(exclusion => exclusion.Id != entity.Id)
            .Where(exclusion => exclusion.CountryId == entity.CountryId)
            .Where(exclusion => exclusion.Date == entity.Date || exclusion.NewDate == entity.Date)
            .AsNoTracking()
            .AnyAsync(cancellationToken);

        var isNewDateExist = await _inquiriesDbContext.CalendarExclusions
            .Where(exclusion => exclusion.Id != entity.Id)
            .Where(exclusion => exclusion.CountryId == entity.CountryId)
            .Where(exclusion => (exclusion.NewDate == entity.NewDate && exclusion.NewDate != null) || exclusion.Date == entity.NewDate)
            .AsNoTracking()
            .AnyAsync(cancellationToken);

        if (isDateExist && isNewDateExist)
        {
            throw new ValidationFailedException("Date and new workday date is already exist. Please check and try again");
        }

        if (isNewDateExist)
        {
            throw new ValidationFailedException("New workday date is already exist. Please check and try again");
        }

        if (isDateExist)
        {
            throw new ValidationFailedException("The date is already exist. Please check and try again");
        }
    }
}
