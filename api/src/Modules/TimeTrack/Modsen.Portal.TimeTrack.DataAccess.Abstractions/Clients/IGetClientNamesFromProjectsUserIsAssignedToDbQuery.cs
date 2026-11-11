using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients;

internal interface IGetClientNamesFromProjectsUserIsAssignedToDbQuery : IDbQuery<int, List<string>>;
