using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Equipment.Requests.Core.BusinessRules;

namespace Modsen.Portal.Equipment.UnitTests.BusinessRule;

public class CanAccessUnassignedEquipmentBusinessRuleTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();

    [Theory]
    [InlineData(PortalRoleNames.Cto, true)]
    [InlineData(PortalRoleNames.Ceo, true)]
    [InlineData(PortalRoleNames.Employee, false)]
    [InlineData(PortalRoleNames.SystemAdministrator, true)]
    public void IsCorrespondsToBusinessWhenRoleSupportedShouldReturnExpectedValue(string roleType, bool expectedResult)
    {
        //Arrange
        var isAvailableEquipmentsWithoutAssignersBusinessRule = new CanAccessUnassignedEquipmentBusinessRule();

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([roleType]);

        //Act
        var result = isAvailableEquipmentsWithoutAssignersBusinessRule.IsCorrespondsToBusiness(_authenticatedUserContext.Object);

        //Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void IsCorrespondsToBusinessWhenAuthedAsHeadOfDepartmentAndIsAvailableToGetEquipmentsWithoutAssigners()
    {
        var isAvailableEquipmentsWithoutAssignersBusinessRule = new CanAccessUnassignedEquipmentBusinessRule();

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContext.Setup(x=>x.HasClaimWithValue(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        //Act
        var result = isAvailableEquipmentsWithoutAssignersBusinessRule.IsCorrespondsToBusiness(_authenticatedUserContext.Object);

        //Assert
        result.Should().Be(true);
    }

    [Fact]
    public void IsCorrespondsToBusinessWhenAuthedAsHeadOfDepartmentAndIsNotAvailableToGetEquipmentsWithoutAssigners()
    {
        var isAvailableEquipmentsWithoutAssignersBusinessRule = new CanAccessUnassignedEquipmentBusinessRule();

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContext.Setup(x=>x.HasClaimWithValue(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        //Act
        var result = isAvailableEquipmentsWithoutAssignersBusinessRule.IsCorrespondsToBusiness(_authenticatedUserContext.Object);

        //Assert
        result.Should().Be(false);
    }

    [Fact]
    public void IsCorrespondsToBusinessWhenRoleNotSupportedShouldThrowApplicationException()
    {
        //Arrange
        var isAvailableEquipmentsWithoutAssignersBusinessRule = new CanAccessUnassignedEquipmentBusinessRule();

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns(["UndefinedRole"]);

        //Act
        var act = () =>
            isAvailableEquipmentsWithoutAssignersBusinessRule.IsCorrespondsToBusiness(
                _authenticatedUserContext.Object);

        //Assert
        act.Should().Throw<InvalidDataAppException>();
    }
}
