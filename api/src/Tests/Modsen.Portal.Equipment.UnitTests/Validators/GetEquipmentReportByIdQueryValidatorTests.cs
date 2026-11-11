using Modsen.Portal.Equipment.Requests.Reports.Requests;

namespace Modsen.Portal.Equipment.UnitTests.Validators;

public class GetEquipmentReportByIdQueryValidatorTests
{
    private readonly GetEquipmentReportByIdRequest.Validator _validator = new();

    [Theory]
    [MemberData(nameof(GetInvalidIds))]
    public void ValidateWhenQueryIsInvalidShouldReturnInvalidResult(int id)
    {
        //Arrange
        var query = new GetEquipmentReportByIdRequest.Query(id);

        //Act
        var result = _validator.Validate(query);

        //Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetValidIds))]
    public void ValidateWhenQueryValidShouldReturnValidResult(int id)
    {
        //Arrange
        var query = new GetEquipmentReportByIdRequest.Query(id);

        //Act
        var result = _validator.Validate(query);

        //Assert
        result.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> GetValidIds()
    {
        yield return [1];
        yield return [101];
    }

    public static IEnumerable<object[]> GetInvalidIds()
    {
        yield return [0];
        yield return [-5];
    }
}
