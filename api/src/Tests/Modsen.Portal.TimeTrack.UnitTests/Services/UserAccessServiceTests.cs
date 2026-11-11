using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
using Modsen.Portal.TimeTrack.UseCases.Services;

namespace Modsen.Portal.TimeTrack.UnitTests.Services;

public class UserAccessServiceTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserMock = new();
    private readonly Mock<IGetTimeTrackUserByIdWithDepartmentDbQuery> _getTimeTrackUserByIdWithDepartmentDbQueryMock = new();
    private readonly UserAccessService _accessService;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UserAccessServiceTests()
    {
        _accessService = new UserAccessService(_authenticatedUserMock.Object, _getTimeTrackUserByIdWithDepartmentDbQueryMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo, null, null)]
    [InlineData(PortalRoleNames.Cto, null, null)]
    [InlineData(PortalRoleNames.HeadOfDepartment, "Net", null)]
    [InlineData(PortalRoleNames.GroupManager, "Net", 1)]
    public async void CanAccessUserReportInformationWhenUserIsInExecutiveRoleAndTriesToGetOtherUserDataValidDataReturnTrue(
        string role, string? departmentId, int? resourceGroupId)
    {
        //Arrange
        var user = new GetTimeTrackUserByIdWithDepartmentDbQueryResponse(
            null, DepartmentId: departmentId, ResourceGroupId: resourceGroupId, null, null, null);

        _authenticatedUserMock.Setup(x => x.GetRoles()).Returns([role]);
        _authenticatedUserMock.Setup(x => x.UserId).Returns(1);
        _authenticatedUserMock.Setup(x => x.DepartmentId).Returns(departmentId);
        _authenticatedUserMock.Setup(x => x.ResourceGroupId).Returns(resourceGroupId);
        _getTimeTrackUserByIdWithDepartmentDbQueryMock.Setup(x => x.ExecuteAsync(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(user);

        //Act
        var result = await _accessService.CanAccessUserReportInformation(99, _cancellationToken);

        //Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(PortalRoleNames.HeadOfDepartment, "Java", null)]
    [InlineData(PortalRoleNames.GroupManager, "Java", 2)]
    public async void CanAccessUserReportInformationWhenUserIsInExecutiveRoleAndTriesToGetOtherUserDataInvalidDataReturnFalse(
        string role, string? departmentId, int? resourceGroupId)
    {
        //Arrange
        var user = new GetTimeTrackUserByIdWithDepartmentDbQueryResponse(
            null, DepartmentId: "Net", ResourceGroupId: 1, null, null, null);

        _authenticatedUserMock.Setup(x => x.GetRoles()).Returns([role]);
        _authenticatedUserMock.Setup(x => x.UserId).Returns(1);
        _authenticatedUserMock.Setup(x => x.DepartmentId).Returns(departmentId);
        _authenticatedUserMock.Setup(x => x.ResourceGroupId).Returns(resourceGroupId);
        _getTimeTrackUserByIdWithDepartmentDbQueryMock.Setup(x => x.ExecuteAsync(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(user);

        //Act
        var result = await _accessService.CanAccessUserReportInformation(99, _cancellationToken);

        //Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    [InlineData(PortalRoleNames.Employee)]
    public async void CanAccessUserReportInformationWhenUserIsNotInExecutiveRoleAndTriesToGetOtherUserDataReturnFalse(string role)
    {
        //Arrange
        _authenticatedUserMock.Setup(x => x.GetRoles()).Returns([role]);
        _authenticatedUserMock.Setup(x => x.UserId).Returns(1);

        //Act
        var result = await _accessService.CanAccessUserReportInformation(99, _cancellationToken);

        //Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.GroupManager)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    [InlineData(PortalRoleNames.Employee)]
    public async void CanAccessUserReportInformationWhenUserTriesToGetHisOwnDataReturnTrue(string role)
    {
        //Arrange
        _authenticatedUserMock.Setup(x => x.GetRoles()).Returns([role]);
        _authenticatedUserMock.Setup(x => x.UserId).Returns(1);

        //Act
        var result = await _accessService.CanAccessUserReportInformation(1, _cancellationToken);

        //Assert
        result.Should().BeTrue();
    }
}
