using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

internal interface IIsProjectExistingByIdDbQuery : IDbQuery<int, bool>;
