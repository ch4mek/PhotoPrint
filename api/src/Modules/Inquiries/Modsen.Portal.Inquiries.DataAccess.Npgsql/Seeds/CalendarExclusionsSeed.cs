using System.Globalization;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Seeds;

internal class CalendarExclusionsSeed : ISeedEntitiesProvider<InquiriesDbContext>
{
    private readonly InquiriesDbContext _dbContext;

    private int _countryId;

    public CalendarExclusionsSeed(InquiriesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private CalendarExclusionEntity Holiday(DateOnly date, DateType type = DateType.Holiday,
        string? name = null)
    {
        if (name == null && type != DateType.Holiday)
        {
            name = $"Transfer {type} from {date.ToString("dd/MM", CultureInfo.InvariantCulture)}";
        }

        var entity = new CalendarExclusionEntity
        {
            Name = name ?? throw new AppException(),
            Date = date,
            DateType = type,
            CountryId = _countryId
        };

        return entity;
    }

    private CalendarExclusionEntity Holiday(DateOnly date, string name)
    {
        return Holiday(date, DateType.Holiday, name);
    }

    private CalendarExclusionEntity[] Holidays()
    {
        return
        [
            Holiday(new DateOnly(2024, 01, 01), "New year"),
            Holiday(new DateOnly(2024, 01, 02), "New year"),
            Holiday(new DateOnly(2024, 01, 07), "Christmas (orthodox)"),
            Holiday(new DateOnly(2024, 03, 08), "Women's Day"),
            Holiday(new DateOnly(2024, 05, 01), "May Day"),
            Holiday(new DateOnly(2024, 05, 09), "Victory Day"),
            Holiday(new DateOnly(2024, 05, 14), "Radonitsa"),
            Holiday(new DateOnly(2024, 07, 03), "Independence Day"),
            Holiday(new DateOnly(2024, 11, 07), "October Revolution Day"),
            Holiday(new DateOnly(2024, 12, 25), "Christmas (catholic)"),
            Holiday(new DateOnly(2025, 01, 01), "New year"),
            Holiday(new DateOnly(2025, 01, 02), "New year"),
            Holiday(new DateOnly(2025, 01, 07), "Christmas (orthodox)"),
            Holiday(new DateOnly(2025, 03, 08), "Women's Day"),
            Holiday(new DateOnly(2025, 04, 29), "May Day"),
            Holiday(new DateOnly(2025, 05, 01), "Victory Day"),
            Holiday(new DateOnly(2025, 05, 09), "Radonitsa"),
            Holiday(new DateOnly(2025, 07, 03), "Independence Day"),
            Holiday(new DateOnly(2025, 11, 07), "October Revolution Day"),
            Holiday(new DateOnly(2025, 12, 25), "Christmas (catholic)"),
        ];
    }

    private void CreateIfNotExist(CalendarExclusionEntity exclusion)
    {
        var hasAny = _dbContext.CalendarExclusions.Any(x =>
            x.Date == exclusion.Date &&
            x.CountryId == exclusion.CountryId);

        if (!hasAny)
        {
            _dbContext.CalendarExclusions.Add(exclusion);
        }
    }

    public void Seed()
    {
        _countryId = 1;

        foreach (var holiday in Holidays())
        {
            CreateIfNotExist(holiday);
        }

        _dbContext.SaveChanges();
    }
}
