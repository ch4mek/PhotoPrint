using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.BusinessRules;

public class CanTrackBillableHoursBusinessRuleTests
{
    private readonly Mock<IIdentityContract> _identityContractMock = new();
    private readonly CanTrackBillableHoursBusinessRule _businessRule;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CanTrackBillableHoursBusinessRuleTests()
    {
        _businessRule = new CanTrackBillableHoursBusinessRule(
            _identityContractMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserHasPermissionAssigmentAndProjectIsBillableShouldReturnSuccess()
    {
        //Arrange
        var projectId = 1;
        var userId = 1;

        var permissionAssigmentDto = new PermissionAssignmentDto
        {
            Id = 1,
            ResourceId = projectId.ToString(),
            UserId = userId,
            ResourceType = PermissionResourceType.Project,
            PermissionType = PermissionType.TrackBillableWorkingHours
        };

        _identityContractMock
            .Setup(x => x.GetPermissionAssignment(
                userId,
                projectId.ToString(),
                PermissionResourceType.Project,
                PermissionType.TrackBillableWorkingHours,
                _cancellationToken))
            .ReturnsAsync(permissionAssigmentDto);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(1, 1, 120, _cancellationToken);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserHasPermissionAssigmentAndProjectIsNotBillableShouldReturnSuccess()
    {
        //Arrange
        var projectId = 1;
        var userId = 1;

        var permissionAssigmentDto = new PermissionAssignmentDto
        {
            Id = 1,
            ResourceId = projectId.ToString(),
            UserId = userId,
            ResourceType = PermissionResourceType.Project,
            PermissionType = PermissionType.TrackBillableWorkingHours
        };

        _identityContractMock
            .Setup(x => x.GetPermissionAssignment(
                userId,
                projectId.ToString(),
                PermissionResourceType.Project,
                PermissionType.TrackBillableWorkingHours,
                _cancellationToken))
            .ReturnsAsync(permissionAssigmentDto);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(1, 1, 120, _cancellationToken);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserHasNoPermissionAssigmentAndProjectIsNotBillableShouldReturnSuccess()
    {
        //Arrange
        var projectId = 1;
        var userId = 1;

        _identityContractMock
            .Setup(x => x.GetPermissionAssignment(
                userId,
                projectId.ToString(),
                PermissionResourceType.Project,
                PermissionType.TrackBillableWorkingHours,
                _cancellationToken))
            .ReturnsAsync(default(PermissionAssignmentDto));

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(1, 1, null, _cancellationToken);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserHasNoPermissionAssigmentAndProjectIsBillableShouldReturnSuccess()
    {
        //Arrange
        var projectId = 1;
        var userId = 1;

        _identityContractMock
            .Setup(x => x.GetPermissionAssignment(
                userId,
                projectId.ToString(),
                PermissionResourceType.Project,
                PermissionType.TrackBillableWorkingHours,
                _cancellationToken))
            .ReturnsAsync(default(PermissionAssignmentDto));

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(1, 1, 150, _cancellationToken);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserHasNoPermissionAssigmentAndHeTriedToTrackHoursWhenProjectIsBillableShouldReturnFail()
    {
        //Arrange
        var projectId = 1;
        var userId = 1;

        _identityContractMock
            .Setup(x => x.GetPermissionAssignment(
                userId,
                projectId.ToString(),
                PermissionResourceType.Project,
                PermissionType.TrackBillableWorkingHours,
                _cancellationToken))
            .ReturnsAsync(default(PermissionAssignmentDto));

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(1, 1, 140, _cancellationToken);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserHasNoPermissionAssigmentAndHeTriedToTrackHoursWhenProjectIsNotBillableShouldReturnFail()
    {
        //Arrange
        var projectId = 1;
        var userId = 1;

        _identityContractMock
            .Setup(x => x.GetPermissionAssignment(
                userId,
                projectId.ToString(),
                PermissionResourceType.Project,
                PermissionType.TrackBillableWorkingHours,
                _cancellationToken))
            .ReturnsAsync(default(PermissionAssignmentDto));

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(1, 1, 140, _cancellationToken);

        //Assert
        result.Should().BeFalse();
    }
}
