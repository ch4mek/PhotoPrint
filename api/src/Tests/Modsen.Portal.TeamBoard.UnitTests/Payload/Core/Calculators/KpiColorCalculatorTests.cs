using Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Payload.KpiColors.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.UnitTests.Payload.Core.Calculators;

public class KpiColorCalculatorTests
{
    private readonly KpiColorCalculator _kpiColorCalculator = new();

    [Theory]
    [MemberData(nameof(GetParameters))]
    public void GetKpiColorShoulReturnValidColor(
        decimal kpi,
        List<KpiColorDto> colors,
        string resultColor)
    {
        //Act
        var result = _kpiColorCalculator.GetKpiColor(kpi, colors);

        //Assert
        result.Should().Be(resultColor);
    }

    private static readonly List<KpiColorDto> KpiColorDtos =
    [
        new() { Id = 1, Name = "Red", RightIntervalBorder = 30 },

        new() { Id = 2, Name = "Orange", RightIntervalBorder = 50 },

        new() { Id = 3, Name = "Green", RightIntervalBorder = 9999 }
    ];

    private static readonly List<KpiColorDto> SecondKpiColorDtos =
    [
        new() { Id = 1, Name = "Red", RightIntervalBorder = 30 },

        new() { Id = 2, Name = "Orange", RightIntervalBorder = 9999 },

        new() { Id = 3, Name = "Green", RightIntervalBorder = 9999 }
    ];

    public static IEnumerable<object[]> GetParameters()
    {
        yield return [0, KpiColorDtos, "Red"];
        yield return [0.5, KpiColorDtos, "Red"];
        yield return [0.999, KpiColorDtos, "Red"];
        yield return [10, KpiColorDtos, "Red"];
        yield return [30, KpiColorDtos, "Red"];
        yield return [30.49, KpiColorDtos, "Red"];
        yield return [30.5, KpiColorDtos, "Orange"];
        yield return [30.9999, KpiColorDtos, "Orange"];
        yield return [31, KpiColorDtos, "Orange"];
        yield return [31.1, KpiColorDtos, "Orange"];
        yield return [45, KpiColorDtos, "Orange"];
        yield return [50.49, KpiColorDtos, "Orange"];
        yield return [50.5, KpiColorDtos, "Green"];
        yield return [50.9999, KpiColorDtos, "Green"];
        yield return [51, KpiColorDtos, "Green"];
        yield return [100, KpiColorDtos, "Green"];
        yield return [9999, KpiColorDtos, "Green"];
        yield return [9999, SecondKpiColorDtos, "Orange"];
        yield return [10000, SecondKpiColorDtos, "Green"];
        yield return [100000, SecondKpiColorDtos, "Green"];
    }
}
