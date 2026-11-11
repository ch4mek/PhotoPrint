namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users.UserNames;

public interface IGetUserNamesResponseByUserIdDbQuery
{
    Task<UserNamesResponse?> GetUserNamesDtoByUserIdAsync(int userId, CancellationToken cancellationToken);
}
