using Modsen.Portal.Merch.Requests.Kits.Requests;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;
using Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;

namespace Modsen.Portal.Merch.UnitTests.Kits.Requests;

public class GetPageableKitsRequestQueryValidatorTests
{
    private readonly GetPageableKitsRequest.Validator _validator = new();

    [Fact]
    public void ValidateWhenTopInQueryIsZeroThrowsException()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto();
        var requestDto = new GetPageableKitsDto { Top = 0, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        //Act
        var result = _validator.Validate(request);

        //Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateWhenSkipInQueryLessThanZeroThrowsException()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto();
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = -1, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        //Act
        var result = _validator.Validate(request);

        //Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateWhenFilterValueIsNullWhenFilterColumnNotNullThrowsException()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto(){ Filters =
        [
            new QueryFilterDto
            {
                FilterColumn = "id",
                FilterValue = null
            }
        ]
        };
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        //Act
        var result = _validator.Validate(request);

        //Assert
        result.IsValid.Should().BeFalse();
    }
}
