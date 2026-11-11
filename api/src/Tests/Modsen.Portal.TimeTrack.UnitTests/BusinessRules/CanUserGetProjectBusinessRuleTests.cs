//using Bogus;
//using Modsen.Portal.Core;
//using Modsen.Portal.Core.Identity;
//using Modsen.Portal.Core.Context;
//using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects.GetProjectWithCreatedByIdAndPublicityAndProjectUsersDepartments;
//using Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;

//namespace Modsen.Portal.TimeTrack.UnitTests.BusinessRules;

//public class CanUserGetProjectBusinessRuleTests
//{
//    private readonly Faker<GetProjectInfoDbQueryModel> _faker = new();

//    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
//    private readonly Mock<IGetProjectInfo>
//        _getProjectWithCreatedByIdAndPublicityAndProjectUsersDepartmentsMock = new();

//    private readonly CanUserGetProjectBusinessRule _businessRule;

//    public CanUserGetProjectBusinessRuleTests()
//    {
//        _businessRule = new CanUserGetProjectBusinessRule(
//            _authenticatedUserContextMock.Object,
//            _getProjectWithCreatedByIdAndPublicityAndProjectUsersDepartmentsMock.Object);

//        _faker.CustomInstantiator(_ => new GetProjectInfoDbQueryModel(false, It.IsAny<int>(),
//            ProjectUsersDepartmentIds: [], ResourceGroupIds: []));
//    }

//    [Theory]
//    [InlineData(PortalRoleNames.Ceo)]
//    [InlineData(PortalRoleNames.Cto)]
//    public async Task IsCorrespondsToBusiness_WhenRoleCanGetAllProjects_ShouldReturnSuccess(string roleType)
//    {
//        // Arrange
//        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([roleType]);

//        // Act
//        var result = await _businessRule.IsCorrespondsToBusiness(1, CancellationToken.None);

//        // Assert
//        result.IsSuccess.Should().BeTrue();
//    }

//    [Fact]
//    public async Task IsCorrespondsToBusiness_WhenRoleIsHeadOfDepartmentAndUserIsProjectCreator_ShouldReturnSuccess()
//    {
//        // Arrange
//        const int headId = 1;

//        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
//        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(headId);
//        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(It.IsAny<string>());

//        _getProjectWithCreatedByIdAndPublicityAndProjectUsersDepartmentsMock
//            .Setup(x => x.ExecuteAsync(headId, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(_faker.Clone().RuleFor(x => x.CreatedById, headId).Generate());

//        // Act
//        var result = await _businessRule.IsCorrespondsToBusiness(headId, CancellationToken.None);

//        // Assert
//        result.IsSuccess.Should().BeTrue();
//    }

//    [Fact]
//    public async Task IsCorrespondsToBusiness_WhenRoleIsHeadOfDepartmentAndProjectIsPublic_ShouldReturnSuccess()
//    {
//        // Arrange
//        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
//        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(It.IsAny<int>());
//        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(It.IsAny<string>());

//        _getProjectWithCreatedByIdAndPublicityAndProjectUsersDepartmentsMock
//            .Setup(x => x.ExecuteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(_faker.Clone().RuleFor(x => x.IsPublic, true).Generate());

//        // Act
//        var result = await _businessRule.IsCorrespondsToBusiness(1, CancellationToken.None);

//        // Assert
//        result.IsSuccess.Should().BeTrue();
//    }

//    [Fact]
//    public async Task
//        IsCorrespondsToBusiness_WhenRoleIsHeadOfDepartmentAndProjectHasUsersFromSameDepartment_ShouldReturnSuccess()
//    {
//        // Arrange
//        const string headDepartmentId = ".Net";

//        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
//        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(It.IsAny<int>());
//        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(headDepartmentId);

//        _getProjectWithCreatedByIdAndPublicityAndProjectUsersDepartmentsMock
//            .Setup(x => x.ExecuteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(_faker.Clone().RuleFor(x => x.ProjectUsersDepartmentIds, [headDepartmentId]).Generate());

//        // Act
//        var result = await _businessRule.IsCorrespondsToBusiness(1, CancellationToken.None);

//        // Assert
//        result.IsSuccess.Should().BeTrue();
//    }

//    [Fact]
//    public async Task IsCorrespondsToBusiness_WhenRoleIsHeadOfDepartment_AndProjectIsPrivate_AndHeadIsNotCreatorOfProject_AndProjectHasNoProjectUsersFromSameDepartment_ShouldReturnFail()
//    {
//        // Arrange
//        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
//        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(It.IsAny<string>());
//        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(1);

//        _getProjectWithCreatedByIdAndPublicityAndProjectUsersDepartmentsMock
//            .Setup(x => x.ExecuteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(_faker.RuleFor(x => x.CreatedById, 2).Generate());

//        // Act
//        var result = await _businessRule.IsCorrespondsToBusiness(1, CancellationToken.None);

//        // Assert
//        result.IsSuccess.Should().BeFalse();
//        result.Reason.Should().Be(PortalMessages.HeadOfDepartmentDoesNotHaveAccessToTheProject);
//    }

//    [Fact]
//    public async Task IsCorrespondsToBusiness_WhenRoleIsUnsupported_ShouldReturnFail()
//    {
//        // Arrange
//        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["unsupported"]);

//        // Act
//        var result = await _businessRule.IsCorrespondsToBusiness(1, CancellationToken.None);

//        // Assert
//        result.IsSuccess.Should().BeFalse();
//        result.Reason.Should().Be("Unsupported RoleTypes");
//    }
//}
