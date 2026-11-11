using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Attendance.DataAccess.Npgsql.OData;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Attendance;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Attendance.Requests.Attendances.Core.Calculators;

internal interface IAttendanceTotalCalculator
{
    public IEnumerable<OdpAttendanceTotal> Calculate(ICollection<AttendanceEntity> attendances);
    public (DateOnly Date, TimeOnly Time) GetLocalDateAndTimeFromCheckedAtUtc(
        DateTime checkedAtUtc,
        LocationType location);
}

internal class AttendanceTotalCalculator : IAttendanceTotalCalculator
{
    public (DateOnly Date, TimeOnly Time) GetLocalDateAndTimeFromCheckedAtUtc(
        DateTime checkedAtUtc,
        LocationType location)
    {
        if (location == LocationType.Unknown)
        {
            return (DateOnly.FromDateTime(checkedAtUtc.Date), TimeOnly.FromDateTime(checkedAtUtc));
        }

        var local = checkedAtUtc.ToLocal(location);
        return (DateOnly.FromDateTime(local.Date), TimeOnly.FromDateTime(local));
    }

    public IEnumerable<OdpAttendanceTotal> Calculate(ICollection<AttendanceEntity> attendances)
    {
        return attendances
            .OrderBy(x => x.UserId)
            .ThenBy(x => x.CheckedAtUtc)
            .GroupBy(x => x.UserId)
            .SelectMany(CalculateTimeIntervals)
            .GroupBy(x => new { x.UserId, x.Date })
            .Select(g => new OdpAttendanceTotal
            {
                UserId = g.Key.UserId,
                Date = g.Key.Date,
                TotalTimeInMinutes = g.Sum(x => x.TotalTimeInMinutes),

                UserDepartmentId = g.First().UserDepartmentId,
                UserFirstName = g.First().UserFirstName,
                UserLastName = g.First().UserLastName,
                UserFirstNameRu = g.First().UserFirstNameRu,
                UserLastNameRu = g.First().UserLastNameRu,
                UserFullName = g.First().UserFullName,
                UserFullNameRu = g.First().UserFullNameRu,
                UserResourceGroupId = g.First().UserResourceGroupId,
                UserResourceGroupName = g.First().UserResourceGroupName,
                IsUserActive = g.First().IsUserActive,
            });
    }

    private static List<OdpAttendanceTotal> CalculateTimeIntervals(IGrouping<int, AttendanceEntity> userAttendances)
    {
        var userTimeIntervals = new List<OdpAttendanceTotal>();
        var enterTimestampLocal = (DateTime?)null;

        foreach (var userAttendance in userAttendances)
        {
            if (userAttendance.Direction == DirectionType.Enter)
            {
                enterTimestampLocal = userAttendance.CheckedAtUtc.ToLocal(userAttendance.Location);
                continue;
            }

            if (!enterTimestampLocal.HasValue)
            {
                continue;
            }

            var exitTimestampLocal = userAttendance.CheckedAtUtc.ToLocal(userAttendance.Location);
            var timeInterval = exitTimestampLocal - enterTimestampLocal!.Value;

            if (timeInterval > TimeSpan.FromHours(PortalSharedConstants.AttendanceMaxHours))
            {
                enterTimestampLocal = null;
                continue;
            }

            if (enterTimestampLocal.Value.Date == exitTimestampLocal.Date)
            {
                AddSingleDayInterval(userAttendance, exitTimestampLocal, timeInterval, userTimeIntervals);
            }
            else
            {
                AddDoubleDayInterval(userAttendance, enterTimestampLocal!.Value, exitTimestampLocal, userTimeIntervals);
            }

            enterTimestampLocal = null;
        }

        return userTimeIntervals;
    }


    private static void AddSingleDayInterval(AttendanceEntity userAttendance, DateTime exitTimestampLocal,
        TimeSpan duration, List<OdpAttendanceTotal> attendanceIntervals)
    {
        var mappedAttendance = OdpAttendanceTotal.Of(userAttendance);
        mappedAttendance.Date = DateOnly.FromDateTime(exitTimestampLocal.Date);
        mappedAttendance.TotalTimeInMinutes = (int)duration.TotalMinutes;
        attendanceIntervals.Add(mappedAttendance);
    }

    private static void AddDoubleDayInterval(AttendanceEntity userAttendance, DateTime enterTimestampLocal,
        DateTime exitTimestampLocal, List<OdpAttendanceTotal> attendanceIntervals)
    {
        // total minutes: enter – start of second day
        var firstDayMappedAttendance = OdpAttendanceTotal.Of(userAttendance);
        firstDayMappedAttendance.Date = DateOnly.FromDateTime(enterTimestampLocal.Date);
        firstDayMappedAttendance.TotalTimeInMinutes = (int)(exitTimestampLocal.Date - enterTimestampLocal).TotalMinutes;

        // total minutes: start of second day – exit
        var secondDayMappedAttendance = OdpAttendanceTotal.Of(userAttendance);
        secondDayMappedAttendance.Date = DateOnly.FromDateTime(exitTimestampLocal.Date);
        secondDayMappedAttendance.TotalTimeInMinutes = exitTimestampLocal.Hour * 60 + exitTimestampLocal.Minute;
        attendanceIntervals.AddRange(firstDayMappedAttendance, secondDayMappedAttendance);
    }
}
