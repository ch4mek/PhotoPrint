using AwesomeAssertions;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.Core.Builders;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.BusinessRules;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Encryptor;
using Modsen.Portal.Finance.Requests.ProjectRates.Requests;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.ProjectRates.Requests;

public class GetProjectRateHistoryRequestTests
{
    private readonly Mock<IProjectRateEncryptor> _projectRateEncryptorMock = new();
    private readonly Mock<IFinanceClientsAndUsersQueryBuilder> _managedClientsQueryBuilderMock = new();
    private readonly Mock<ICanFinanceUserEditProjectRatesBusinessRule> _canFinanceUserEditProjectRatesBusinessRuleMock = new();
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetProjectRateHistoryRequest _request;

    public GetProjectRateHistoryRequestTests()
    {
        _request = new GetProjectRateHistoryRequest(
            _projectRateEncryptorMock.Object,
            _managedClientsQueryBuilderMock.Object,
            _canFinanceUserEditProjectRatesBusinessRuleMock.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenProjectNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        _dbContextMock.SetupGet(x => x.FinanceProjects)
            .Returns(new List<FinanceProjectEntity>().BuildMockDbSet().Object);

        // Act
        var action = () => _request.Handle(new GetProjectRateHistoryRequest.Query(1, 1), _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.ProjectNotFound);
    }

    [Fact]
    public async Task HandleWhenUserNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        var project = TestDataGenerator.GetFinanceProjectEntity(TestDataGenerator.GetFinanceClientEntity());

        _dbContextMock.SetupGet(x => x.FinanceProjects)
            .Returns(new[] { project }.BuildMockDbSet().Object);

        _dbContextMock.SetupGet(x => x.FinanceUsers)
            .Returns(new List<FinanceUserEntity>().BuildMockDbSet().Object);

        // Act
        var action = () => _request.Handle(new GetProjectRateHistoryRequest.Query(1, project.Id), _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenUserHasNoAccessShouldThrowActionNotAllowedException()
    {
        // Arrange
        var project = TestDataGenerator.GetFinanceProjectEntity(TestDataGenerator.GetFinanceClientEntity());
        var user = TestDataGenerator.GetFinanceUserEntity(id: 1);

        _dbContextMock.SetupGet(x => x.FinanceProjects)
            .Returns(new[] { project }.BuildMockDbSet().Object);

        _dbContextMock.SetupGet(x => x.FinanceUsers)
            .Returns(new[] { user }.BuildMockDbSet().Object);

        _managedClientsQueryBuilderMock
            .Setup(x => x.BuildAvailableFinanceUserIdsAsync(It.IsAny<CancellationToken>(), 0, 0))
            .ReturnsAsync(new[] { 2, 3 }.AsAsyncQueryable());

        // Act
        var action = () => _request.Handle(new GetProjectRateHistoryRequest.Query(user.Id, project.Id), _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenNoRatesShouldReturnEmptyCollection()
    {
        // Arrange
        var project = TestDataGenerator.GetFinanceProjectEntity(TestDataGenerator.GetFinanceClientEntity());
        var user = TestDataGenerator.GetFinanceUserEntity();

        _dbContextMock.SetupGet(x => x.FinanceProjects)
            .Returns(new[] { project }.BuildMockDbSet().Object);

        _dbContextMock.SetupGet(x => x.FinanceUsers)
            .Returns(new[] { user }.BuildMockDbSet().Object);

        _dbContextMock.SetupGet(x => x.ProjectRates)
            .Returns(new List<DataAccess.Npgsql.Entities.ProjectRateEntity>().BuildMockDbSet().Object);

        _managedClientsQueryBuilderMock
            .Setup(x => x.BuildAvailableFinanceUserIdsAsync(It.IsAny<CancellationToken>(), 0, 0))
            .ReturnsAsync(new[] { user.Id }.AsAsyncQueryable());

        // Act
        var result = await _request.Handle(new GetProjectRateHistoryRequest.Query(user.Id, project.Id), _cancellationToken);

        // Assert
        result.ProjectRates.Should().BeEmpty();
        result.ProjectId.Should().Be(project.Id);
        result.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task HandleWhenRatesExistShouldReturnOrderedHistory()
    {
        // Arrange
        var project = TestDataGenerator.GetFinanceProjectEntity(TestDataGenerator.GetFinanceClientEntity());
        var user = TestDataGenerator.GetFinanceUserEntity();

        var rates = new List<DataAccess.Npgsql.Entities.ProjectRateEntity>
        {
            TestDataGenerator.GetProjectRateEntity(id: 1, user.Id, project.Id, new DateOnly(2025, 1, 1), encrypted: "enc1"),
            TestDataGenerator.GetProjectRateEntity(id: 2, user.Id, project.Id, new DateOnly(2026, 2, 1), encrypted: "enc2"),
            TestDataGenerator.GetProjectRateEntity(id: 3, user.Id, project.Id, new DateOnly(2026, 3, 1), encrypted: "enc3"),
        };

        var expManagementResult = rates.ToDictionary(x => new ProjectRateBusinessKey(x.UserId, x.ProjectId, x.RateFrom), _ => true);

        _dbContextMock.SetupGet(x => x.FinanceProjects)
            .Returns(new[] { project }.BuildMockDbSet().Object);

        _dbContextMock.SetupGet(x => x.FinanceUsers)
            .Returns(new[] { user }.BuildMockDbSet().Object);

        _dbContextMock.SetupGet(x => x.ProjectRates)
            .Returns(rates.BuildMockDbSet().Object);

        _managedClientsQueryBuilderMock
            .Setup(x => x.BuildAvailableFinanceUserIdsAsync(
                It.IsAny<CancellationToken>(),
                0,
                0))
            .ReturnsAsync(new[] { user.Id }.AsAsyncQueryable());

        _canFinanceUserEditProjectRatesBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusinessBatch(
                It.IsAny<IEnumerable<ProjectRateBusinessKey>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expManagementResult);

        _projectRateEncryptorMock.Setup(x => x.Decrypt("enc1")).Returns(100m);
        _projectRateEncryptorMock.Setup(x => x.Decrypt("enc2")).Returns(200m);
        _projectRateEncryptorMock.Setup(x => x.Decrypt("enc3")).Returns(300m);

        // Act
        var result = await _request.Handle(new GetProjectRateHistoryRequest.Query(user.Id, project.Id), _cancellationToken);

        // Assert
        result.ProjectRates.Should().HaveCount(3);

        result.ProjectRates.Select(x => x.RateFrom)
            .Should()
            .BeInAscendingOrder();

        result.ProjectRates[0].HourlyRate.Should().Be(100m);
        result.ProjectRates[1].HourlyRate.Should().Be(200m);
        result.ProjectRates[2].HourlyRate.Should().Be(300m);
    }
}
