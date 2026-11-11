using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.BusinessRules;

public class HasUserActiveAssignmentForDateBusinessRuleTests
{
    private readonly Mock<IEnvironmentFeatureContext> _featureContextMock = new();
    private readonly Mock<ITimeTrackDbContext> _dbContextMock = new();

    private readonly HasUserActiveAssignmentForDateBusinessRule _businessRule;
    private readonly CancellationToken _token = CancellationToken.None;

    public HasUserActiveAssignmentForDateBusinessRuleTests()
    {
        _businessRule = new HasUserActiveAssignmentForDateBusinessRule(
            _featureContextMock.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenProjectIsPublicAndFeatureIsEnabledReturnsTrue()
    {
        //Arrange
        const int projectId = 1;
        const int userId = 1;
        var reportForDate = DateOnly.FromDateTime(DateTime.Today);

        _featureContextMock
            .Setup(context => context.IsCheckAssignDatesFeatureEnabledAsync())
            .ReturnsAsync(true);

        _dbContextMock
            .Setup(context => context.Projects)
            .Returns(new List<Project>
            {
                new()
                {
                    Id = projectId,
                    IsPublic = true,
                    Name = "test"
                }
            }.BuildMockDbSet().Object);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(projectId, userId, reportForDate, _token);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenFeatureIsDisabledAndProjectIsPrivateReturnsTrue()
    {
        //Arrange
        const int projectId = 1;
        const int userId = 1;
        var reportForDate = DateOnly.FromDateTime(DateTime.Today);

        _featureContextMock
            .Setup(context => context.IsCheckAssignDatesFeatureEnabledAsync())
            .ReturnsAsync(false);

        _dbContextMock
            .Setup(context => context.Projects)
            .Returns(new List<Project>
            {
                new()
                {
                    Id = projectId,
                    IsPublic = false,
                    Name = "test"
                }
            }.BuildMockDbSet().Object);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(projectId, userId, reportForDate, _token);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserIsOnPrivateProjectAndFeatureEnabledReturnsTrue()
    {
        //Arrange
        const int projectId = 1;
        const int userId = 1;
        var reportForDate = DateOnly.FromDateTime(DateTime.Today);

        _featureContextMock
            .Setup(context => context.IsCheckAssignDatesFeatureEnabledAsync())
            .ReturnsAsync(true);

        _dbContextMock
            .Setup(context => context.Projects)
            .Returns(new List<Project>
            {
                new()
                {
                    Id = projectId,
                    Name = "test",
                    IsPublic = false,
                    ProjectUsers =
                    [
                        new ProjectUser
                        {
                            UserId = userId,
                            UserOnProjectFrom = reportForDate.AddDays(-10)
                        }
                    ]
                }
            }.BuildMockDbSet().Object);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(projectId, userId, reportForDate, _token);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserIsNotOnPrivateProjectAndFeatureEnabledReturnsFalse()
    {
        //Arrange
        const int projectId = 1;
        const int userId = 1;
        var reportForDate = DateOnly.FromDateTime(DateTime.Today);

        _featureContextMock
            .Setup(context => context.IsCheckAssignDatesFeatureEnabledAsync())
            .ReturnsAsync(true);

        _dbContextMock
            .Setup(context => context.Projects)
            .Returns(new List<Project>
            {
                new()
                {
                    Id = projectId,
                    Name = "test",
                    IsPublic = false,
                    ProjectUsers =
                    [
                        new ProjectUser
                        {
                            UserId = userId,
                            UserOnProjectFrom = reportForDate.AddDays(-10),
                            UserOnProjectTo = reportForDate.AddDays(-1)
                        }
                    ]
                }
            }.BuildMockDbSet().Object);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(projectId, userId, reportForDate, _token);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenProjectNotFoundReturnsFalse()
    {
        //Arrange
        const int projectId = 1;
        const int userId = 1;
        var reportForDate = DateOnly.FromDateTime(DateTime.Today);

        _featureContextMock
            .Setup(context => context.IsCheckAssignDatesFeatureEnabledAsync())
            .ReturnsAsync(true);

        _dbContextMock
            .Setup(context => context.Projects)
            .Returns(new List<Project>
            {
                new()
                {
                    Id = projectId + 1,
                    Name = "test",
                    IsPublic = false
                }
            }.BuildMockDbSet().Object);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(projectId, userId, reportForDate, _token);

        //Assert
        result.Should().BeFalse();
    }
}
