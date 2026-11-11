using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;

public interface IGetUsersWithInactiveTimeTrackingDbQuery
    : IDbQuery<GetUsersWithInactiveTimeTrackingDbRequest, List<GetUsersWithInactiveTimeTrackingDbResponse>>;

public record GetUsersWithInactiveTimeTrackingDbRequest(DateOnly StartDate, DateOnly EndDate);

public record GetUsersWithInactiveTimeTrackingDbResponse(string Email, string Name);
