using Modsen.Portal.TeamBoard.Requests.Payload.KpiColors.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;

internal interface IKpiColorCalculator
{
    string GetKpiColor(decimal kpi, List<KpiColorDto> colors);
}

internal class KpiColorCalculator : IKpiColorCalculator
{
    public string GetKpiColor(decimal kpi, List<KpiColorDto> colors)
    {
        var colorName = colors[^1].Name;
        var leftBorder = 0;

        for (var i = 1; i < colors.Count; i++)
        {
            var roundedKpi = (int)Math.Round(kpi, MidpointRounding.AwayFromZero);

            var isGreaterThanOrEqualLeftBorder = leftBorder <= roundedKpi;
            var isLessThanRightBorder = roundedKpi < colors[i - 1].RightIntervalBorder + 1;

            var isInInterval = isGreaterThanOrEqualLeftBorder && isLessThanRightBorder;

            if (isInInterval)
            {
                colorName = colors[i - 1].Name;
            }

            leftBorder = colors[i - 1].RightIntervalBorder + 1;
        }

        return colorName;
    }
}
