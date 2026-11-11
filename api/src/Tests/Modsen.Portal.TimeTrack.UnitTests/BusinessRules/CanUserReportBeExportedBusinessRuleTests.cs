using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;

namespace Modsen.Portal.TimeTrack.UnitTests.BusinessRules;

public class CanUserReportBeExportedBusinessRuleTests
{
    private readonly Mock<IGetTimeTrackUserByIdWithDepartmentDbQuery> _getTimeTrackUserByIdWithDepartmentDbQueryMock = new();
    private readonly Mock<IAuthenticatedUserContext> _userContextMock = new();

    private readonly CanUserReportBeExportedBusinessRule _businessRule;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CanUserReportBeExportedBusinessRuleTests()
    {
        _businessRule = new CanUserReportBeExportedBusinessRule(
            _getTimeTrackUserByIdWithDepartmentDbQueryMock.Object,
            _userContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserNotExistsShouldThrowEntityNorFoundException()
    {
        // Arrange
        var userId = 1;

        _getTimeTrackUserByIdWithDepartmentDbQueryMock
            .Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(new GetTimeTrackUserByIdWithDepartmentDbQueryResponse(null, null, null, null, null, null));

        // Act
        var actionUnderTest = () => _businessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenHeadIsFromAnotherDepartmentShouldReturnFailed()
    {
        // Arrange
        var userId = 1;
        var user = new GetTimeTrackUserByIdWithDepartmentDbQueryResponse(UserId:userId, DepartmentId: "Net", null, null, null, null);

        _getTimeTrackUserByIdWithDepartmentDbQueryMock
            .Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _userContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _userContextMock.Setup(x => x.DepartmentId).Returns("Qa");

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().Match<BusinessRuleResult>(x => !x.IsSuccess &&
        x.Reason == PortalMessages.HeadCantExportReportOfUserFromOtherDepartment);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenGmIsFromAnotherGroupShouldReturnFailed()
    {
        // Arrange
        var userId = 1;
        var user = new GetTimeTrackUserByIdWithDepartmentDbQueryResponse(UserId: userId, DepartmentId: "Net", ResourceGroupId: 1, null, null, null);

        _getTimeTrackUserByIdWithDepartmentDbQueryMock
            .Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _userContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _userContextMock.Setup(x => x.DepartmentId).Returns("Java");
        _userContextMock.Setup(x => x.ResourceGroupId).Returns(2);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().Match<BusinessRuleResult>(x => !x.IsSuccess &&
            x.Reason == PortalMessages.GroupManagerCantExportReportOfUserFromOtherGroup);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCurrentUserCanExportReportShouldReturnSuccess()
    {
        // Arrange
        var userId = 1;

        var user = new GetTimeTrackUserByIdWithDepartmentDbQueryResponse(UserId: 1, DepartmentId: "Net", null, null, null, null);

        _getTimeTrackUserByIdWithDepartmentDbQueryMock
            .Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _userContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().Match<BusinessRuleResult>(x => x.IsSuccess && x.Reason == null);
    }
}
