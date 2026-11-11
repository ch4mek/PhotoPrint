using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Attendance;
using Modsen.Portal.Core.Services.Cqrs;

namespace Modsen.Portal.Attendance.Requests.Jobs;

internal sealed class SyncAttendancesJob : ICommandHandler<SyncAttendancesJob.Command, Unit>
{
    private readonly IAttendanceDbContext _attendanceDbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SyncAttendancesJob> _logger;

    private const string _getAccessEventsQuery =
        """
        SELECT TOP 100 [Id], [AuthenticationDateTime], [Direction], [DeviceName], [PersonName], [IsSync]
        FROM [dbo].[AccessEvents]
        WHERE [PersonName] LIKE '%@modsen-software.com'
        AND [IsSync] = 0
        """;

    private static readonly Func<int[], string> _updateAccessEventsCommand = ids =>
        $"""
         UPDATE [dbo].[AccessEvents]
         SET [IsSync] = 1
         WHERE [Id] IN ({string.Join(',', ids)})
         """;

    public SyncAttendancesJob(IAttendanceDbContext attendanceDbContext, IConfiguration configuration,
        ILogger<SyncAttendancesJob> logger)
    {
        _attendanceDbContext = attendanceDbContext;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        var connectionString = _configuration["ConnectionStrings:AccessEventsDb"];

        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        var attendanceUsers = await _attendanceDbContext.Users
            .ToDictionaryAsync(x => x.Email, x => x.UserId, StringComparer.InvariantCultureIgnoreCase, cancellationToken);

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var selectCommand = new SqlCommand(_getAccessEventsQuery, connection);
        await using var accessEventsReader = await selectCommand.ExecuteReaderAsync(cancellationToken);

        var attendancesToSync = new List<AttendanceEntity>();

        while (await accessEventsReader.ReadAsync(cancellationToken))
        {

            var id = (int)accessEventsReader["Id"];
            var authenticationDateTime = (DateTime)accessEventsReader["AuthenticationDateTime"];
            var direction = (string)accessEventsReader["Direction"];
            var deviceName = (string)accessEventsReader["DeviceName"];
            var personName = (string)accessEventsReader["PersonName"];
            var doorLocation = deviceName.Split('_');

            if (attendanceUsers.TryGetValue(personName, out var userId))
            {
                var attendance = new AttendanceEntity
                {
                    Id = id,
                    CheckedAtUtc = ConvertToUtc(authenticationDateTime),
                    Direction = Enum.Parse<DirectionType>(direction),
                    Door = doorLocation[0],
                    Location = Enum.Parse<LocationType>(doorLocation[1]),
                    UserId = userId
                };

                attendancesToSync.Add(attendance);
            }
            else
            {
                _logger.LogError("Attendances: user {PersonName} is not found", personName);
            }
        }

        await accessEventsReader.CloseAsync();

        var syncedAttendanceIds = attendancesToSync.Select(x => x.Id).ToArray();
        if (syncedAttendanceIds.Length == 0)
        {
            return Unit.Value;
        }

        await _attendanceDbContext.Attendances.AddRangeAsync(attendancesToSync, cancellationToken);        
        await using var updateCommand = new SqlCommand(_updateAccessEventsCommand(syncedAttendanceIds), connection);
        await _attendanceDbContext.SaveChangesAsync(cancellationToken);
        await updateCommand.ExecuteNonQueryAsync(cancellationToken);
        await connection.CloseAsync();

        return Unit.Value;
    }


    // it is required to specify datetime as utc
    private static DateTime ConvertToUtc(DateTime dateTime)
    {
        var date = DateOnly.FromDateTime(dateTime);
        var time = TimeOnly.FromDateTime(dateTime);

        return new DateTime(date, time, DateTimeKind.Utc);
    }

    internal sealed record Command : ICommand<Unit>, ITransactionalRequest;
}
