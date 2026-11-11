using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Reports.Requests;
using Modsen.Portal.Equipment.Requests.Reports.Requests.RequestDtos;

namespace Modsen.Portal.Equipment.UnitTests.Requests;

public class GetEquipmentReportByIdRequestTests
{
    private readonly Mock<IEquipmentDbContext> _equipmentDbContextMock = new();
    private readonly CancellationToken _cancellationToken;
    private readonly GetEquipmentReportByIdRequest _handler;

    public GetEquipmentReportByIdRequestTests()
    {
        _cancellationToken = CancellationToken.None;
        _handler = new GetEquipmentReportByIdRequest(_equipmentDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenEquipmentReportDoesNotExistShouldThrowEntityNotFoundException()
    {
        //Arrange
        var equipmentReportId = 1;
        var request = new GetEquipmentReportByIdRequest.Query(equipmentReportId);

        SetupEquipmentReports([]);

        //Act
        var result = () => _handler.Handle(request, _cancellationToken);

        //Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenEquipmentReportExistsShouldReturnEquipmentReportDto()
    {
        //Arrange
        var equipmentReportId = 2;
        var request = new GetEquipmentReportByIdRequest.Query(equipmentReportId);

        var expected = new EquipmentReportDto
        {
            Id = 2,
            SerialNumber = "GKSLAVWQ",
            Data = "{\"data\": \"info\"}",
            AssembledAtUtc = DateTime.UtcNow,
            CreatedAtUtc = DateTime.UtcNow
        };

        var equipmentReport = new EquipmentReport
        {
            Id = expected.Id,
            SerialNumber = expected.SerialNumber,
            Data = expected.Data,
            DataHash = string.Empty,
            AssembledAtUtc = expected.AssembledAtUtc,
            CreatedAtUtc = expected.CreatedAtUtc
        };

        SetupEquipmentReports([equipmentReport]);

        //Act
        var result = await _handler.Handle(request, _cancellationToken);

        //Assert
        result.Should().BeEquivalentTo(expected);
    }

    private void SetupEquipmentReports(List<EquipmentReport> equipmentReports)
    {
        _equipmentDbContextMock
            .Setup(mock => mock.EquipmentReports)
            .Returns(equipmentReports.BuildMockDbSet().Object);
    }
}
