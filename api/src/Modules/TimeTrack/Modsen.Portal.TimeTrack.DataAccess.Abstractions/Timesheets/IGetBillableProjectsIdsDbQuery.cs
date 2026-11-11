using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;

internal interface IGetBillableProjectsIdsDbQuery : IDbQuery<List<int>, List<int>>;
