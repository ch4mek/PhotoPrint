using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.Validators;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.HardSkills.Core;

public class HardSkillUpdateValidatorTests
{
    private readonly Mock<IIsHardSkillNameUniqueBusinessRule> _isHardSkillNameUniqueBusinessRule = new();
    private readonly Mock<IHardSkillGroupExistsBusinessRule> _hardSkillGroupExistsBusinessRule = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly HardSkillUpdateValidator _validator;

    public HardSkillUpdateValidatorTests()
    {
        _validator = new HardSkillUpdateValidator(_isHardSkillNameUniqueBusinessRule.Object,
            _hardSkillGroupExistsBusinessRule.Object);
    }

    [Fact]
    public async Task ValidateAsyncWhenHardSkillNameUniqueThrowsInvalidDataAppException()
    {
        //Arrange
        var id = 1;
        var name = "SkillName";
        var groupId = "GroupID";

        _isHardSkillNameUniqueBusinessRule.Setup(x =>
                x.IsCorrespondsToBusiness(id, name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _hardSkillGroupExistsBusinessRule.Setup(x =>
                x.IsCorrespondsToBusiness(groupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        //Act
        var result = () => _validator.ValidateAsync(id, name, groupId, _cancellationToken);

        //Assert
        await result.Should().ThrowAsync<InvalidDataAppException>("Hard skill with the same name already exists");

    }

    [Fact]
    public async Task ValidateAsyncWhenHardSkillGroupDoesNotExistThrowsInvalidDataAppException()
    {
        //Arrange
        var id = 1;
        var name = "SkillName";
        var groupId = "GroupID";

        _isHardSkillNameUniqueBusinessRule.Setup(x =>
                x.IsCorrespondsToBusiness(id, name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _hardSkillGroupExistsBusinessRule.Setup(x =>
                x.IsCorrespondsToBusiness(groupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        //Act
        var result = () => _validator.ValidateAsync(id, name, groupId, _cancellationToken);

        //Assert
        await result.Should().ThrowAsync<InvalidDataAppException>("There is no hard skill group with such id");

    }

    [Fact]
    public async Task ValidateAsyncWhenHardSkillGroupExistsAndNameUniqueNotThrowsAnyException()
    {
        //Arrange
        var id = 1;
        var name = "SkillName";
        var groupId = "GroupID";

        _isHardSkillNameUniqueBusinessRule.Setup(x =>
                x.IsCorrespondsToBusiness(id, name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _hardSkillGroupExistsBusinessRule.Setup(x =>
                x.IsCorrespondsToBusiness(groupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        //Act
        var result = () => _validator.ValidateAsync(id, name, groupId, _cancellationToken);

        //Assert
        await result.Should().NotThrowAsync();

    }
}
