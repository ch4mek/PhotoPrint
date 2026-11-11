namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;

internal interface IUpdateIsActiveForUserDbQuery
{
    Task Set(int userId, bool isActive, CancellationToken cancellationToken);
}
