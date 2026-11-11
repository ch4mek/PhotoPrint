using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;

internal interface ITimeTrackUserProvider
{
    Task<TimeTrackUser?> GetByIdAsync(
        int userId,
        CancellationToken cancellationToken);

    Task<List<TimeTrackUser>> GetLookupWithParametersAsync(
        FilterParams filterParams,
        string? keyWord,
        CancellationToken token);

    Task<List<TimeTrackUser>> GetLookupWithParametersForReportsAsync(
        FilterParams filterParams,
        bool isClientWithPublicProject,
        bool isPublicProject,
        bool isAnyPublicProjectManagedByUser,
        string? keyWord,
        CancellationToken token);
}
