using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;

public interface IIsTimeTrackUserExistingByUserIdDbQuery : IDbQuery<int, bool>;
