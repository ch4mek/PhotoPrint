using AwesomeAssertions;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.BusinessRules;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Composers;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Loaders;
using Modsen.Portal.Finance.Requests.ProjectRates.Requests;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.ProjectRates.Requests;

public class CreateProjectRateRequestTests
{
    private readonly Mock<IIsProjectRateUniqueForDateBusinessRule> _isProjectRateUniqueForDateBusinessRuleMock = new();
    private readonly Mock<IProjectUserLoader> _projectUserLoaderMock = new();
    private readonly Mock<ICanFinanceUserEditProjectRatesBusinessRule> _canFinanceUserEditProjectRateBusinessRuleMock = new();
    private readonly Mock<IProjectRateComposer> _projectRateComposerMock = new ();
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly CreateProjectRateRequest _createProjectRateRequest;

    public CreateProjectRateRequestTests()
    {
        _createProjectRateRequest = new CreateProjectRateRequest(
            _isProjectRateUniqueForDateBusinessRuleMock.Object,
            _projectUserLoaderMock.Object,
            _canFinanceUserEditProjectRateBusinessRuleMock.Object,
            _projectRateComposerMock.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        _dbContextMock.SetupGet(x => x.FinanceUsers)
            .Returns(new List<FinanceUserEntity>().BuildMockDbSet().Object);

        // Act
        var action = () => _createProjectRateRequest.Handle(GetCommand(), _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenProjectNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        var command = GetCommand();

        _dbContextMock.SetupGet(x => x.FinanceUsers)
            .Returns(new[] { TestDataGenerator.GetFinanceUserEntity() }.BuildMockDbSet().Object);

        _dbContextMock.SetupGet(x => x.FinanceProjects)
            .Returns(new List<FinanceProjectEntity>().BuildMockDbSet().Object);

        // Act
        var action = () => _createProjectRateRequest.Handle(GetCommand(), _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.ProjectNotFound);
    }

    [Fact]
    public async Task HandleWhenProjectIsPublicThrowActionNotAllowedException()
    {
        // Arrange
        var command = GetCommand();
        var project = TestDataGenerator.GetFinanceProjectEntity(TestDataGenerator.GetFinanceClientEntity(), isPublic: true);

        _dbContextMock.SetupGet(x => x.FinanceUsers)
            .Returns(new[] { TestDataGenerator.GetFinanceUserEntity() }.BuildMockDbSet().Object);

        _dbContextMock.SetupGet(x => x.FinanceProjects)
            .Returns(new List<FinanceProjectEntity> { project }.BuildMockDbSet().Object);

        // Act
        var action = () => _createProjectRateRequest.Handle(GetCommand(), _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.ProjectRatesOnlyForPrivateProjects);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotHaveProjectAccessThrowInvalidDataAppException()
    {
        // Arrange
        var command = GetCommand();
        SetupUserAndProject();

        _projectUserLoaderMock
            .Setup(x => x.GetBillableUserProjectOnSpecificMonthAsync(command.UserId, command.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<int>());

        // Act
        var action = () => _createProjectRateRequest.Handle(GetCommand(), _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.UserDoesNotHaveAccessToSomeProjects);
    }

    [Fact]
    public async Task HandleWhenRateDateConflictsThrowEntityConflictException()
    {
        // Arrange
        var command = GetCommand();
        SetupUserAndProject();

        _projectUserLoaderMock
            .Setup(x => x.GetBillableUserProjectOnSpecificMonthAsync(command.UserId, command.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync([command.ProjectId]);

        _isProjectRateUniqueForDateBusinessRuleMock
            .Setup(x => x.IsInvalid(null, command.UserId, command.ProjectId, command.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var action = () => _createProjectRateRequest.Handle(command, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityConflictException>()
            .WithMessage(PortalMessages.ProjectRateDateConflicts);
    }

    [Fact]
    public async Task HandleWhenUserCannotEditRatesThrowActionNotAllowedException()
    {
        // Arrange
        var command = GetCommand();
        SetupUserAndProject();

        _projectUserLoaderMock
            .Setup(x => x.GetBillableUserProjectOnSpecificMonthAsync(command.UserId, command.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync([command.ProjectId]);

        _isProjectRateUniqueForDateBusinessRuleMock
            .Setup(x => x.IsInvalid(null, command.UserId, command.ProjectId, command.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _canFinanceUserEditProjectRateBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(command.UserId, command.ProjectId, command.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var action = () => _createProjectRateRequest.Handle(command, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenRequestIsValidCreateRate()
    {
        // Arrange
        var command = GetCommand();
        var expextedCreateRateCoreDto = new CreateProjectRateCoreDto(command.UserId, command.ProjectId, command.HourlyRate, command.CurrencyId, command.RateFrom);

        SetupUserAndProject();

        _projectUserLoaderMock
            .Setup(x => x.GetBillableUserProjectOnSpecificMonthAsync(command.UserId, command.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync([command.ProjectId]);

        _isProjectRateUniqueForDateBusinessRuleMock
            .Setup(x => x.IsInvalid(null, command.UserId, command.ProjectId, command.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _canFinanceUserEditProjectRateBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(command.UserId, command.ProjectId, command.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _createProjectRateRequest.Handle(command, _cancellationToken);

        // Assert
        _projectRateComposerMock.Verify(
            x => x.ComposeAsync(expextedCreateRateCoreDto, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private CreateProjectRateRequest.Command GetCommand()
    {
        return new CreateProjectRateRequest.Command
        {
            UserId = 1,
            ProjectId = 1,
            HourlyRate = 1,
            CurrencyId = CurrencyCodes.USDollar,
            RateFrom = new DateOnly(2025, 1, 1)
        };
    }

    private void SetupUserAndProject()
    {
        var client = TestDataGenerator.GetFinanceClientEntity();

        _dbContextMock.SetupGet(x => x.FinanceUsers)
            .Returns(new[] { TestDataGenerator.GetFinanceUserEntity() }.BuildMockDbSet().Object);

        _dbContextMock.SetupGet(x => x.FinanceProjects)
            .Returns(new[] { TestDataGenerator.GetFinanceProjectEntity(client) }.BuildMockDbSet().Object);
    }
}
