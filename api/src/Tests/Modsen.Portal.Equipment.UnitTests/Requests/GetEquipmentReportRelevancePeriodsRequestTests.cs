using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Reports.Requests;

namespace Modsen.Portal.Equipment.UnitTests.Requests;

public class GetEquipmentReportRelevancePeriodsRequestTests
{
    private readonly CancellationToken _cancellationToken;
    private readonly Mock<IEquipmentDbContext> _equipmentDbContextMock = new();
    private readonly GetEquipmentReportRelevancePeriodsRequest _handler;

    public GetEquipmentReportRelevancePeriodsRequestTests()
    {
        _cancellationToken = CancellationToken.None;
        _handler = new GetEquipmentReportRelevancePeriodsRequest(
            _equipmentDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenEquipmentReportRelevancePeriodsCountIsZeroShouldThrowEntityNotFoundException()
    {
        //Arrange
        var serialNumber = "ISKASFJQ";
        var request = new GetEquipmentReportRelevancePeriodsRequest.Query(serialNumber);

        SetupEquipmentReportRelevancePeriods([]);

        //Act
        var result = () => _handler.Handle(request, _cancellationToken);

        //Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    private void SetupEquipmentReportRelevancePeriods(List<EquipmentReportRelevancePeriod> periods)
    {
        _equipmentDbContextMock
            .Setup(mock => mock.EquipmentReportRelevancePeriods)
            .Returns(periods.BuildMockDbSet().Object);
    }
}
