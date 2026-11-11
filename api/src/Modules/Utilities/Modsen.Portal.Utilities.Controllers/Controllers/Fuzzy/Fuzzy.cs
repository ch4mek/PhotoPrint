namespace Modsen.Portal.Utilities.Controllers.Fuzzy;

public class Fuzzy
{
    public string? IssueToCompare { get; set; }
    public double DiceCoefficient { get; set; }
    public string? DiceCoefficientFormatted { get; set; }
    public decimal LevenshteinDistanceValue { get; set; }
    public decimal LevenshteinDistancePercent { get; set; }
    public string? LevenshteinDistanceFormatted { get; set; }
    public double LongestCommonSubsequence { get; set; }
    public string? LongestCommonSubsequenceFormatted { get; set; }
}
