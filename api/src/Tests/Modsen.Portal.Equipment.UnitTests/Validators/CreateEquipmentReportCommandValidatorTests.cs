using Microsoft.AspNetCore.Http;
using Modsen.Portal.Equipment.Requests.Reports.Requests;

namespace Modsen.Portal.Equipment.UnitTests.Validators;

public class CreateEquipmentReportCommandValidatorTests
{
    private readonly CreateEquipmentReportRequest.Validator _validator = new();

    [Theory]
    [MemberData(nameof(GetInvalidFileData))]
    public void ValidateWhenCommandIsInvalidShouldReturnInvalidResult(
        string jsonData,
        string fileName)
    {
        //Arrange
        var formFileMock = new Mock<IFormFile>();

        formFileMock.Setup(formFile => formFile.FileName)
                    .Returns(fileName);

        var command = new CreateEquipmentReportRequest.Command(formFileMock.Object);

        //Act
        var result = _validator.Validate(command);

        //Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetValidJsonData))]
    public void ValidateWhenCommandIsValidShouldReturnValidResult(
        string jsonData,
        string fileName)
    {
        //Arrange
        var formFileMock = new Mock<IFormFile>();

        formFileMock.Setup(formFile => formFile.FileName)
                    .Returns(fileName);

        var command = new CreateEquipmentReportRequest.Command(formFileMock.Object);

        //Act
        var result = _validator.Validate(command);

        //Assert
        result.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> GetValidJsonData()
    {
        yield return
        [
            "{\"data\": \"info\", \"CreationDate\": \"2024-01-25 05.25\"," +
                       "\"HardwareSerialNumber\": \"SKVKAFA\"}",
            "info.json"
        ];
        yield return
        [
            "\"CreationDate\": \"2024-01-25 05.25\"," +
                       "\"HardwareSerialNumber\": \"SKVKAFA\"}",
            "data.json"
        ];
    }

    public static IEnumerable<object[]> GetInvalidFileData()
    {
        yield return ["example string", "info.txt"];
        yield return ["465231854", "data.txt"];
    }
}
