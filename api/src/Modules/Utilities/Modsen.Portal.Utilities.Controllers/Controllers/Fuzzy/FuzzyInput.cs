namespace Modsen.Portal.Utilities.Controllers.Fuzzy;

public class FuzzyInput
{
    public string? Issue { get; set; }
    public bool? IsCaseSensitive { get; set; } = false;
    public IReadOnlyCollection<string> IssuesToCompare { get; set; } = [];
}
