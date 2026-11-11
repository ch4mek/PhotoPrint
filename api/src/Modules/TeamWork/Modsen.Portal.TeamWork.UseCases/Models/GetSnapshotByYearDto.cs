namespace Modsen.Portal.TeamWork.UseCases.Models;

public record GetSnapshotByYearDto(int StartYear, int EndYear, string? DepartmentId, IReadOnlyCollection<int> Projects, IReadOnlyCollection<int> Clients);
