using System.Collections.ObjectModel;

namespace Modsen.Portal.Utilities.Controllers.Fuzzy;

public class FuzzyOutput
{
    public string? Issue { get; set; }
    public int LengthOfIssue { get; set; }
    public bool IsCaseSensitive { get; set; }
    public Collection<Fuzzy> IssuesToCompare { get; } = [];
}
