using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Utilities.Controllers.Fuzzy;

[AllowAnonymous]
[Area("[Shared] Fuzzy")]
[Route("api/fuzzy")]
public class FuzzyController : Controller
{
    [HttpPost("compare")]
    public IActionResult Compare([FromBody] FuzzyInput input, [FromHeader(Name = "X-FuzzyCompare-Key")] string fuzzyCompareKey)
    {
        if (fuzzyCompareKey.IsNullOrWhiteSpace() || fuzzyCompareKey != "V6e84Tza6Wg8csAr")
        {
            return Unauthorized();
        }

        ArgumentNullException.ThrowIfNull(input);

        var output = new FuzzyOutput
        {
            LengthOfIssue = input.Issue?.Length ?? 0,
            Issue = input.Issue ?? string.Empty,
            IsCaseSensitive = input.IsCaseSensitive ?? false
        };

        if (output.LengthOfIssue > 0)
        {
            foreach (var issue in input.IssuesToCompare)
            {
                var fuzzy = new Fuzzy
                {
                    IssueToCompare = issue,
                    DiceCoefficient = Math.Round(input.Issue.DiceCoefficient(issue), 5)
                };

                fuzzy.DiceCoefficientFormatted = $"{fuzzy.DiceCoefficient:###,###.00000}";

                fuzzy.LevenshteinDistanceValue = input.Issue.LevenshteinDistance(issue, input.IsCaseSensitive ?? false);
                fuzzy.LevenshteinDistancePercent = Math.Round(fuzzy.LevenshteinDistanceValue * 100m / (output.LengthOfIssue * 0.7m), 5);
                fuzzy.LevenshteinDistanceFormatted = $"{fuzzy.LevenshteinDistancePercent:###,###.00000}";

                var longestSubSeq = input.Issue.LongestCommonSubsequence(issue, input.IsCaseSensitive ?? false);
                fuzzy.LongestCommonSubsequence = Math.Round(longestSubSeq.Item2, 5);
                fuzzy.LongestCommonSubsequenceFormatted = $"{fuzzy.LongestCommonSubsequence:###,###.00000}";

                output.IssuesToCompare.Add(fuzzy);
            }
        }

        return Ok(output);
    }
}
