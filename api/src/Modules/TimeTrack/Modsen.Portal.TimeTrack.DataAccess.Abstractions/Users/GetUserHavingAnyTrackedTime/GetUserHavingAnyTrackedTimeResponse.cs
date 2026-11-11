namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users.GetUserHavingAnyTrackedTime;

internal record GetUserHavingAnyTrackedTimeResponse(int UserId, string? DepartmentId, int? ResourceGroupId);
