using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;

namespace Modsen.Portal.TimeTrack.UnitTests.BusinessRules;

public class GetAccessedDepartmentIdForCurrentUserBusinessRuleTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly IGetAccessedDepartmentIdForCurrentUserBusinessRule _businessRule;
    private readonly CancellationToken _token = CancellationToken.None;

    public GetAccessedDepartmentIdForCurrentUserBusinessRuleTests()
    {
        _businessRule = new GetAccessedDepartmentIdForCurrentUserBusinessRule(_authenticatedUserContextMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task CheckAccessWhenAuthAsCEOorCTOWithoutDepartmentShouldReturnNULL(string role)
    {
        //Arrange
        _authenticatedUserContextMock.Setup(a => a.GetRoles()).Returns([role]);

        //Act
        var result = await _businessRule.Execute(null, _token);

        //Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task CheckAccessWhenAuthAsCEOorCTOWithDepartmentShouldReturnDepartment(string role)
    {
        //Arrange
        _authenticatedUserContextMock.Setup(a => a.GetRoles()).Returns([role]);

        //Act
        var result = await _businessRule.Execute("Net", _token);

        //Assert
        result.Should().Be("Net");
    }

    [Theory]
    [InlineData("Net")]
    [InlineData("Js")]
    [InlineData(null)]
    public async Task CheckAccessWhenAuthAsHeadWithDepartmentShouldReturnCorrectDepartment(string? departmentId)
    {
        //Arrange
        _authenticatedUserContextMock.Setup(a => a.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(a => a.DepartmentId).Returns("Net");

        //Act
        var result = await _businessRule.Execute(departmentId, _token);

        //Assert
        result.Should().Be("Net");
    }

    [Theory]
    [InlineData("Net")]
    [InlineData("Js")]
    [InlineData(null)]
    public async Task CheckAccessWhenAuthAsSystemAdministratorWithDepartmentShouldReturnCorrectDepartment(string? departmentId)
    {
        //Arrange
        _authenticatedUserContextMock.Setup(a => a.GetRoles()).Returns([PortalRoleNames.SystemAdministrator]);
        _authenticatedUserContextMock.Setup(a => a.DepartmentId).Returns("Js");

        //Act
        var result = await _businessRule.Execute(departmentId, _token);

        //Assert
        result.Should().Be("Js");
    }

    [Theory]
    [InlineData("Net")]
    [InlineData("Js")]
    [InlineData(null)]
    public async Task CheckAccessWhenAuthAsEmployeeShouldReturnCorrectDepartment(string? departmentId)
    {
        //Arrange
        _authenticatedUserContextMock.Setup(a => a.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(a => a.DepartmentId).Returns("Net");

        //Act
        var result = await _businessRule.Execute(departmentId, _token);

        //Assert
        result.Should().Be("Net");
    }
}
