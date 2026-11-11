using System.Globalization;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Attendance;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.Attendance.DataAccess.Npgsql.Seeds;

internal class AttendancesSeed : ISeedEntitiesProvider<AttendanceDbContext>
{
    private readonly AttendanceDbContext _context;
    private readonly ILogger<AttendancesSeed> _logger;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;

    public AttendancesSeed(AttendanceDbContext context, ILogger<AttendancesSeed> logger,
        IHostEnvironmentContext hostEnvironmentContext)
    {
        _context = context;
        _logger = logger;
        _hostEnvironmentContext = hostEnvironmentContext;
    }

    public void Seed()
    {
        if (!IsSeedableEnvironment())
        {
            return;
        }

        var seeds = GetSeedingAttendances();

        _context.Attendances.AddRange(seeds);

        _context.SaveChanges();
    }

    private HashSet<int> GetExistingAttendancesIds()
    {
        return _context.Attendances.Select(x => x.Id).ToHashSet();
    }

    private IEnumerable<AttendanceEntity> GetSeedingAttendances()
    {
        var existingAttendances = GetExistingAttendancesIds();
        var csvPath = GetPathToCsv();

        if (!File.Exists(csvPath))
        {
            _logger.LogWarning("There is no csv file with attendances seed at {Path}", csvPath);

            return [];
        }

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

        var csvRecords = csv.GetRecords<AttendanceCsv>().Where(x => !existingAttendances.Contains(x.Id)).ToList();
        var csvUserEmails = csvRecords.Select(x => x.UserEmail).Distinct();

        var usersIds = _context.Users
            .Where(user => csvUserEmails.Contains(user.Email))
            .ToDictionary(user => user.Email, user => user.UserId);

        return csvRecords.Select(x => new AttendanceEntity
        {
            Id = x.Id,
            Door = x.Door,
            UserId = usersIds[x.UserEmail],
            CheckedAtUtc = x.CheckedAt.ToUniversalTime(),
            Location = Enum.Parse<LocationType>(x.Location),
            Direction = Enum.Parse<DirectionType>(x.Direction)
        });
    }

    private static string GetPathToCsv()
    {
        var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
        return Path.Combine(executingDirectory, "Seeds", "Csv", "Attendances.csv");
    }

    private bool IsSeedableEnvironment()
    {
        return _hostEnvironmentContext.IsDevelopmentEnvironment() ||
            _hostEnvironmentContext.IsQaEnvironment() ||
            _hostEnvironmentContext.IsStagingEnvironment();
    }

    private record AttendanceCsv(
        int Id,
        DateTime CheckedAt,
        string Location,
        string Door,
        string Direction,
        string UserEmail);
}
