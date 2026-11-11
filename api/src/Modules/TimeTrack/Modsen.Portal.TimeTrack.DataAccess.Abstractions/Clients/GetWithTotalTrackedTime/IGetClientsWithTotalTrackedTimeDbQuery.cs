using Modsen.Portal.Core.Services;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients.GetTotalTrackedTime;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients.GetWithTotalTrackedTime;

public interface IGetClientsWithTotalTrackedTimeDbQuery
    : IDbQuery<GetClientsWithTotalTrackedTimeDbRequest, List<ClientWithTotalTrackedTimeModel>>;
