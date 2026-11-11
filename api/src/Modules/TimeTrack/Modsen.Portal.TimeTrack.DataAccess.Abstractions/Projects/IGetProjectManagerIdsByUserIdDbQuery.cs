using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

internal interface IGetProjectManagerIdsByUserIdDbQuery : IDbQuery<int, List<int>>;
