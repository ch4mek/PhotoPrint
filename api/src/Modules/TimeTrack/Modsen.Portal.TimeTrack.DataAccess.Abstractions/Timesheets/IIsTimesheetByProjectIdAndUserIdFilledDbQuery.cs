using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;

internal interface IIsTimesheetByProjectIdAndUserIdFilledDbQuery : IDbQuery<IsTimesheetFilledModel, bool>;

internal record IsTimesheetFilledModel(int UserId, int ProjectId);
