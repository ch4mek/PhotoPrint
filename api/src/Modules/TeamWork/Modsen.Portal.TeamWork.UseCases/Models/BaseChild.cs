namespace Modsen.Portal.TeamWork.UseCases.Models;

public abstract class BaseChild
{
    public long Duration { get; set; }
    public string? Name { get; set; }
    public IReadOnlyCollection<object> Amounts { get; set; } = [];
}
