namespace Modsen.Portal.TeamWork.UseCases.Models;

public class GetSnapshotByYearResponse<T>
    where T : BaseChild
{
    public IReadOnlyCollection<Total> Totals { get; set; } = [];
    public IReadOnlyCollection<Group<T>> GroupOne { get; set; } = [];
}
