using Modsen.Portal.Equipment.Requests.Reports.Requests;

namespace Modsen.Portal.Equipment.UnitTests.Validators;

public class GetEquipmentReportRelevancePeriodsQueryValidatorTests
{
    private readonly GetEquipmentReportRelevancePeriodsRequest.Validator _validator = new();

    [Theory]
    [MemberData(nameof(GetInvalidSerialNumbers))]
    public void ValidateWhenQueryIsInvalidShouldReturnInvalidResult(string serialNumber)
    {
        //Arrange
        var query = new GetEquipmentReportRelevancePeriodsRequest.Query(serialNumber);

        //Act
        var result = _validator.Validate(query);

        //Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetValidSerialNumbers))]
    public void ValidateWhenQueryValidShouldReturnValidResult(string serialNumber)
    {
        //Arrange
        var query = new GetEquipmentReportRelevancePeriodsRequest.Query(serialNumber);

        //Act
        var result = _validator.Validate(query);

        //Assert
        result.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> GetValidSerialNumbers()
    {
        yield return ["SKVKAFA"];
        yield return ["PASMVQW"];
    }

    public static IEnumerable<object[]> GetInvalidSerialNumbers()
    {
        yield return [""];
    }
}
