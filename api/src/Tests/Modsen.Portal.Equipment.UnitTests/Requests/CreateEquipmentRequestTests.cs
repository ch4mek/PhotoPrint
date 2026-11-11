using System.Text;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Reports.Requests;
using Modsen.Portal.Equipment.Requests.Reports.Requests.RequestDtos;

namespace Modsen.Portal.Equipment.UnitTests.Requests;

public class CreateEquipmentRequestTests
{
    private readonly Mock<IEquipmentDbContext> _equipmentDbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly Mock<IHashService> _hashServiceMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly CreateEquipmentReportRequest _handler;

    public CreateEquipmentRequestTests()
    {
        _handler = new CreateEquipmentReportRequest(
            _equipmentDbContextMock.Object,
            _clockServiceMock.Object,
            _hashServiceMock.Object);
    }

    [Fact]
    public async Task HandleWhenHardwareSerialNumberNotFoundShouldThrowInvalidDataAppException()
    {
        //Arrange
        var jsonData = "{\"data\": \"info\", \"CreationDate\": \"2024-01-29T06:51:24Z\"}";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));

        var formFileMock = new Mock<IFormFile>();

        formFileMock
            .Setup(formFile => formFile.OpenReadStream())
            .Returns(stream);

        var request = new CreateEquipmentReportRequest.Command(formFileMock.Object);

        //Act
        var result = () => _handler.Handle(request, _cancellationToken);

        //Assert
        await result.Should().ThrowAsync<InvalidDataAppException>();

        formFileMock.Verify();
    }

    [Fact]
    public async Task HandleWhenEquipmentIsNotExistingShouldThrowInvalidDataAppException()
    {
        //Arrange
        var jsonData = "{\"data\": \"info\", \"CreationDate\": \"2024-01-29T06:51:24Z\"," +
                       "\"HardwareSerialNumber\": \"SKVKAFA\"}";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));

        var formFileMock = new Mock<IFormFile>();

        formFileMock
            .Setup(formFile => formFile.OpenReadStream())
            .Returns(stream);

        var request = new CreateEquipmentReportRequest.Command(formFileMock.Object);

        SetupEquipment([]);

        //Act
        var result = () => _handler.Handle(request, _cancellationToken);

        //Assert
        await result.Should().ThrowAsync<InvalidDataAppException>();

        formFileMock.Verify();
    }

    [Fact]
    public async Task HandleWhenCreationDateNotFoundShouldThrowInvalidDataAppException()
    {
        //Arrange
        var jsonData = "{\"data\": \"info\", \"HardwareSerialNumber\": \"SKVKAFA\"}";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));

        var formFileMock = new Mock<IFormFile>();

        formFileMock
            .Setup(formFile => formFile.OpenReadStream())
            .Returns(stream);

        var equipments = new List<EquipmentEntity>()
        {
            new()
            {
                Id = 1,
                SerialNumber = "SKVKAFA",
                BrokenStatus = EquipmentBrokenStatus.None,
                Name = "Laptop",
                PurchasePlace = "Amazon",
                Characteristics = "16GB RAM"
            }
        };

        SetupEquipment(equipments);

        var request = new CreateEquipmentReportRequest.Command(formFileMock.Object);

        //Act
        var result = () => _handler.Handle(request, _cancellationToken);

        //Assert
        await result.Should().ThrowAsync<InvalidDataAppException>();

        formFileMock.Verify();
    }

    [Fact]
    public async Task HandleWhenEquipmentReportNotFoundShouldReturnEquipmentReportDto()
    {
        //Arrange
        var serialNumber = "SKVKAFA";
        var creationDate = DateTime.Parse("2024-01-29T06:51:24Z").ToUniversalTime();

        var jsonData = "{\"data\": \"info\", \"CreationDate\": \"2024-01-29T06:51:24Z\"," +
                       "\"HardwareSerialNumber\": \"SKVKAFA\"}";

        var resultJsonData = "{\"data\":\"info\"}";

        var jsonDataHash = "somehash";

        var equipments = new List<EquipmentEntity>()
        {
            new()
            {
                Id = 1,
                SerialNumber = "SKVKAFA",
                BrokenStatus = EquipmentBrokenStatus.None,
                Name = "Laptop",
                PurchasePlace = "Amazon",
                Characteristics = "16GB RAM"
            }
        };

        SetupEquipment(equipments);
        SetupGetHash(jsonDataHash, resultJsonData);
        SetupEquipmentReports([]);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));

        var formFileMock = new Mock<IFormFile>();

        formFileMock
            .Setup(formFile => formFile.OpenReadStream())
            .Returns(stream);

        var request = new CreateEquipmentReportRequest.Command(formFileMock.Object);

        var equipmentReport = new EquipmentReport
        {
            Id = 5,
            Data = resultJsonData,
            DataHash = jsonDataHash,
            SerialNumber = serialNumber.ToLower(),
            AssembledAtUtc = creationDate,
            CreatedAtUtc = DateTime.UtcNow
        };

        var equipmentReportDto = new EquipmentReportDto
        {
            Id = 5,
            Data = resultJsonData,
            SerialNumber = serialNumber.ToLower(),
            AssembledAtUtc = creationDate,
            CreatedAtUtc = equipmentReport.CreatedAtUtc,
        };

        //Act
        var result = await _handler.Handle(request, _cancellationToken);

        //Assert
        result.SerialNumber.Should().Be(equipmentReportDto.SerialNumber);
        result.Data.Should().Be(equipmentReportDto.Data);
        result.AssembledAtUtc.Should().Be(equipmentReportDto.AssembledAtUtc);

        formFileMock.Verify();
        _hashServiceMock.Verify();
    }

    private void SetupEquipment(List<EquipmentEntity> equipments)
    {
        _equipmentDbContextMock
            .Setup(mock => mock.Equipments)
            .Returns(equipments.BuildMockDbSet().Object);
    }

    private void SetupGetHash(string hash, string resultJsonData)
    {
        _hashServiceMock
            .Setup(hashService => hashService.GetSha256Hash(resultJsonData))
            .Returns(hash);
    }

    private void SetupEquipmentReports(List<EquipmentReport> equipmentReports)
    {
        _equipmentDbContextMock
            .Setup(mock => mock.EquipmentReports)
            .Returns(equipmentReports.BuildMockDbSet().Object);
    }
}
