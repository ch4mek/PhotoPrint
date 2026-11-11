using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients;

internal interface IGetResourceGroupIdsByClientDbQuery : IDbQuery<int, List<int>>;
