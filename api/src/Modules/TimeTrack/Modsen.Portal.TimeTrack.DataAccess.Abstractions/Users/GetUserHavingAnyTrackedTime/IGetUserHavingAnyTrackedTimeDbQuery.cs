using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users.GetUserHavingAnyTrackedTime;

internal interface IGetUserHavingAnyTrackedTimeDbQuery : IDbQuery<int, GetUserHavingAnyTrackedTimeResponse?>;
