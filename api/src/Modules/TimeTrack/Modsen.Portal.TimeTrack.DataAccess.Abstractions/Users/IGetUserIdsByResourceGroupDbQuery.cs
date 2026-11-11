using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;

internal interface IGetUserIdsByResourceGroupDbQuery : IDbQuery<int, List<int>>;
