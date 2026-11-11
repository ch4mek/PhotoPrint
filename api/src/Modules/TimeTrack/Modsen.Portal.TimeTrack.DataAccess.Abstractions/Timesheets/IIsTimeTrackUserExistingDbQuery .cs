using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;

internal interface IIsTimeTrackUserExistingDbQuery : IDbQuery<int, bool>;
