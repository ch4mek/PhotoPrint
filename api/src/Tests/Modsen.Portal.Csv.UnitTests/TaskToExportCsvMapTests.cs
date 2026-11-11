using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Csv.Models;

namespace Modsen.Portal.Csv.UnitTests;

public class TaskToExportCsvMapTests
{
    private readonly string _defaultDateFormat = "dd/MM/yyyy";

    [Fact]
    public void CtorShouldMapAllProperties()
    {
        // Arrange & Act
        var map = new TaskToExportCsvMap(_defaultDateFormat);

        // Assert
        map.MemberMaps.All(memberMap => memberMap.Data.Ignore).Should().BeFalse();
        map.MemberMaps.SelectMany(memberMap => memberMap.Data.Names.Names).Should().Contain("Time (h)");
        map.MemberMaps.FirstOrDefault(memberMap => memberMap.Data.Member?.Name == nameof(TaskToCsvExportModel.TimeInMinutes))
            .Should().NotBeNull();
        map.MemberMaps.FirstOrDefault(memberMap => memberMap.Data.Member?.Name == nameof(TaskToCsvExportModel.Date))
            .Should().NotBeNull();
        map.MemberMaps.FirstOrDefault(memberMap => memberMap.Data.Member?.Name == nameof(TaskToCsvExportModel.Description))
            .Should().NotBeNull();
    }
}
