using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;

internal interface ICreateOrUpdateUserDbQuery
{
    Task Execute(TimeTrackUser timeTrackUser, string? position, CancellationToken cancellationToken);
}
