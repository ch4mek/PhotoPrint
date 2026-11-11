using System.Linq.Expressions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Jira.Contract;
using Modsen.Portal.Jira.Contract.Exceptions;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Dtos;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core.Composers;
using Modsen.Portal.TimeTrack.UseCases.Jira.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Jira.Core.Requests;

public class ImportJiraTimesheetRequestTests
{
    private readonly Mock<ITimeTrackDbContext> _dbContextMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();
    private readonly Mock<IConfiguration> _configurationMock = new();
    private readonly Mock<IJiraSettingsDataProtectionFactory> _protectionFactoryMock = new();
    private readonly Mock<IDataProtectionService> _dataProtectionServiceMock = new();
    private readonly Mock<IBackgroundJobManager> _backgroundJobManagerMock = new();
    private readonly Mock<IJiraService> _jiraServiceMock = new();
    private readonly Mock<IJiraTimesheetImportComposer> _jiraTimesheetImportComposerMock = new();
    private readonly Mock<ILogger<ImportJiraTimesheetRequest>> _loggerMock = new();
    private readonly Mock<IConnectionFactory> _connectionFactoryMock = new();

    private readonly ImportJiraTimesheetRequest _request;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ImportJiraTimesheetRequestTests()
    {
        _protectionFactoryMock
            .Setup(x => x.Create())
            .Returns(_dataProtectionServiceMock.Object);

        _request = new ImportJiraTimesheetRequest(
            _dbContextMock.Object,
            _identityContractMock.Object,
            _configurationMock.Object,
            _protectionFactoryMock.Object,
            _backgroundJobManagerMock.Object,
            _jiraServiceMock.Object,
            _jiraTimesheetImportComposerMock.Object,
            _loggerMock.Object,
            _connectionFactoryMock.Object);
    }

    [Fact]
    public async Task HandleShouldReturnUnit()
    {
        _identityContractMock
            .Setup(x => x.GetUsersByClaim(
                JiraClaimTypeNames.ViewJiraImport.Name,
                _cancellationToken))
            .ReturnsAsync([1]);

        _configurationMock
            .Setup(x => x["AppSettings:JiraSettings:OneTimesheetImportQueue"])
            .Returns("jira-import");

        _dbContextMock
            .Setup(x => x.JiraSettings)
            .Returns(new List<JiraSetting>().BuildMockDbSet().Object);

        var result = await _request.Handle(
            new ImportJiraTimesheetRequest.Command(new DateOnly(2025, 1, 1)), _cancellationToken);

        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task HandleShouldScheduleJobForEveryJiraSetting()
    {
        _identityContractMock
            .Setup(x => x.GetUsersByClaim(
                JiraClaimTypeNames.ViewJiraImport.Name,
                _cancellationToken))
            .ReturnsAsync([1]);

        _configurationMock
            .Setup(x => x["AppSettings:JiraSettings:OneTimesheetImportQueue"])
            .Returns("jira-import");

        _dbContextMock
            .Setup(x => x.JiraSettings)
            .Returns(new List<JiraSetting>
            {
                new()
                {
                    Id = 1,
                    UserId = 1,
                    IsActive = true,
                    User = new TimeTrackUser { IsActive = true, Email = "user@email.com,", FirstName = "Test", LastName = "Test" },
                    Project = new Project { IsActive = true, Name = "test" },
                    Url = "http//test.url",
                    Pat = "secret pat",
                    JiraProjectTag = "TEST"
                },
                new()
                {
                    Id = 2,
                    UserId = 1,
                    IsActive = true,
                    User = new TimeTrackUser { IsActive = true, Email = "user@email.com,", FirstName = "Test", LastName = "Test"  },
                    Project = new Project { IsActive = true, Name = "test" },
                    Url = "http//test.url",
                    Pat = "secret pat",
                    JiraProjectTag = "TEST"
                }
            }.BuildMockDbSet().Object);

        await _request.Handle(
            new ImportJiraTimesheetRequest.Command(
                new DateOnly(2025, 1, 1)),
            _cancellationToken);

        _backgroundJobManagerMock.Verify(
            x => x.Schedule(
                "jira-import",
                It.IsAny<Expression<Func<Task>>>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task ImportTimesheetsShouldThrowEntityNotFoundExceptionWhenSettingNotFound()
    {
        _dbContextMock
            .Setup(x => x.JiraSettings)
            .Returns(new List<JiraSetting>().BuildMockDbSet().Object);

        var action = () => _request.ImportTimesheets(1, new DateOnly(2025, 1, 1),_cancellationToken);

        await action.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task ImportTimesheetsShouldReturnWhenPatIsEmpty()
    {
        SetupJiraSetting();

        _dataProtectionServiceMock
            .Setup(x => x.Decrypt(It.IsAny<string>()))
            .Returns(string.Empty);

        await _request.ImportTimesheets(1, new DateOnly(2025, 1, 1), _cancellationToken);

        _jiraServiceMock.Verify(
            x => x.GetWorklogsAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ImportTimesheetsShouldRetur_WhenJiraThrowsUnexpectedException()
    {
        SetupJiraSetting();

        _dataProtectionServiceMock
            .Setup(x => x.Decrypt(It.IsAny<string>()))
            .Returns("pat");

        _jiraServiceMock
            .Setup(x => x.GetWorklogsAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException());

        await _request.ImportTimesheets(1, new DateOnly(2025, 1, 1), _cancellationToken);

        _jiraTimesheetImportComposerMock.Verify(
            x => x.Compose(
                It.IsAny<IReadOnlyCollection<JiraImportedWorklog>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ImportTimesheetsShouldRethrowWhenJiraTemporarilyUnavailable()
    {
        SetupJiraSetting();

        _dataProtectionServiceMock
            .Setup(x => x.Decrypt(It.IsAny<string>()))
            .Returns("pat");

        _jiraServiceMock
            .Setup(x => x.GetWorklogsAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new JiraTemporarilyUnavailableException());

        var action = () => _request.ImportTimesheets(1, new DateOnly(2025, 1, 1), _cancellationToken);

        await action.Should()
            .ThrowAsync<JiraTemporarilyUnavailableException>();
    }

    [Fact]
    public async Task ImportTimesheetsShouldSaveAndCommitWhenImportSucceeded()
    {
        SetupJiraSetting();

        _dataProtectionServiceMock
            .Setup(x => x.Decrypt(It.IsAny<string>()))
            .Returns("pat");

        _jiraServiceMock
            .Setup(x => x.GetWorklogsAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _jiraTimesheetImportComposerMock
            .Setup(x => x.Compose(
                It.IsAny<IReadOnlyCollection<JiraImportedWorklog>>(),
                1,
                100,
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new JiraImportResultDto
            {
                Created = 1,
                Updated = 2,
                Skipped = 3,
                Total = 6,
            });

        _loggerMock
            .Setup(x => x.IsEnabled(LogLevel.Information))
            .Returns(true);

        await _request.ImportTimesheets(1, new DateOnly(2025, 1, 1), _cancellationToken);

        _dbContextMock.Verify(
            x => x.SaveChanges(_cancellationToken),
            Times.Once);

        _connectionFactoryMock.Verify(
            x => x.CommitCurrentTransaction(_cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ImportTimesheetsShouldNotSaveWhenComposeThrowsActionNotAllowedException()
    {
        SetupJiraSetting();

        _dataProtectionServiceMock
            .Setup(x => x.Decrypt(It.IsAny<string>()))
            .Returns("pat");

        _jiraServiceMock
            .Setup(x => x.GetWorklogsAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _jiraTimesheetImportComposerMock
            .Setup(x => x.Compose(
                It.IsAny<IReadOnlyCollection<JiraImportedWorklog>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ActionNotAllowedException());

        await _request.ImportTimesheets(
            1,
            new DateOnly(2025, 1, 1),
            _cancellationToken);

        _dbContextMock.Verify(
            x => x.SaveChanges(It.IsAny<CancellationToken>()),
            Times.Never);

        _connectionFactoryMock.Verify(
            x => x.CommitCurrentTransaction(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private void SetupJiraSetting()
    {
        _dbContextMock
            .Setup(x => x.JiraSettings)
            .Returns(new List<JiraSetting>
            {
                new()
                {
                    Id = 1,
                    Url = "https://jira.company.com",
                    Pat = "encryptedPat",
                    JiraProjectTag = "TEST",
                    UserId = 1,
                    ProjectId = 100,
                    Email = "user@test.com"
                }
            }.BuildMockDbSet().Object);
    }
}
