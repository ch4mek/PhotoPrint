using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamWork.UseCases.Models;

namespace Modsen.Portal.TeamWork.UseCases.GetSnapshotByYear;

internal sealed record GetSnapshotByYearQuery(int StartYear, int EndYear, string? DepartmentId, IReadOnlyCollection<int> Projects, IReadOnlyCollection<int> Clients)
    : IQuery<GetSnapshotByYearResponse<UserChild>>;
