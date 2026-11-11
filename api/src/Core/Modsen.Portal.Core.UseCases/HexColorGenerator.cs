using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Core.UseCases;

public sealed class HexColorGenerator : IHexColorGenerator
{
    private static readonly List<string> s_colors =
    [
        "#f7c531",
        "#b320ab",
        "#ce0e5b",
        "#4ea5e3",
        "#1aaf82",
        "#008744",
        "#0057e7",
        "#d62d20",
        "#ffa700",
        "#800080",
        "#be29ec",
        "#adff00",
        "#00ff83",
        "#f47835",
        "#433e90",
        "#fdff32",
        "#6f0000",
        "#18fff9",
        "#ff084a",
        "#35a79c"
    ];

    public string GenerateColor()
    {
        var colorIndex = Random.Shared.Next(0, s_colors.Count);
        
        return s_colors[colorIndex];
    }
}
