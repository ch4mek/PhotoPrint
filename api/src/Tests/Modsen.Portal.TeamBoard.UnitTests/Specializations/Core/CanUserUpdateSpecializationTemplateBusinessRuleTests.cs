using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Specializations.Core;

public class CanUserUpdateSpecializationTemplateBusinessRuleTests
{
    private readonly Mock<IGetSpecializationTemplateDepartmentsDbQuery>
        _getSpecializationTemplateDepartmentsDbQuery = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly CanUserUpdateSpecializationTemplateBusinessRule _businessRule;

    public CanUserUpdateSpecializationTemplateBusinessRuleTests()
    {
        _businessRule = new CanUserUpdateSpecializationTemplateBusinessRule(
            _authenticatedUserContext.Object,
            _getSpecializationTemplateDepartmentsDbQuery.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task IsCorrespondsToBusinessWhenUserIsCeoOrCtoReturnsTrue(string roleType)
    {
        //Arrange
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([roleType]);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(1, 1, _token);

        //Assert
        result.Should().BeTrue();

    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task IsCorrespondsToBusinessWhenUserIsEmployeeOrSysAdminReturnsFalse(string roleType)
    {
        //Arrange
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([roleType]);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(1, 1, _token);

        //Assert
        result.Should().BeFalse();

    }

    [Theory]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.GroupManager)]
    public async Task IsCorrespondsToBusinessWhenUserIsGmOrHeadAndCreatedTemplateReturnsTrue(string roleType)
    {
        //Arrange
        const int creatorId = 1;

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([roleType]);
        _authenticatedUserContext.Setup(context => context.UserId).Returns(creatorId);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(creatorId, 1, _token);

        //Assert
        result.Should().BeTrue();

    }

    [Theory]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.GroupManager)]
    public async Task IsCorrespondsToBusinessWhenGmOrHeadAndTemplateDepartmentsNotContainTheirDepartmentReturnsFalse(
        string roleType)
    {
        //Arrange
        const int creatorId = 1;
        const int templateId = 1;
        const string departmentId = "Net";

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([roleType]);
        _authenticatedUserContext.Setup(context => context.UserId).Returns(creatorId + 1);
        _authenticatedUserContext.Setup(context => context.DepartmentId).Returns(departmentId);

        _getSpecializationTemplateDepartmentsDbQuery
            .Setup(dbQuery => dbQuery.ExecuteAsync(templateId, _token))
            .ReturnsAsync([
                new() { DepartmentId = "Java" },
                new() { DepartmentId = "JS" },
                new() { DepartmentId = "PMO" }
            ]);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(creatorId, templateId, _token);

        //Assert
        result.Should().BeFalse();

    }

    [Theory]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.GroupManager)]
    public async Task IsCorrespondsToBusinessWhenGmOrHeadAndTemplateDepartmentsContainTheirDepartmentReturnsTrue(
        string roleType)
    {
        //Arrange
        const int creatorId = 1;
        const int templateId = 1;
        const string departmentId = "Net";

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([roleType]);
        _authenticatedUserContext.Setup(context => context.UserId).Returns(creatorId + 1);
        _authenticatedUserContext.Setup(context => context.DepartmentId).Returns(departmentId);

        _getSpecializationTemplateDepartmentsDbQuery
            .Setup(dbQuery => dbQuery.ExecuteAsync(templateId, _token))
            .ReturnsAsync([
                new() { DepartmentId = "Java" },
                new() { DepartmentId = "JS" },
                new() { DepartmentId = departmentId }
            ]);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(creatorId, templateId, _token);

        //Assert
        result.Should().BeTrue();

    }
}
