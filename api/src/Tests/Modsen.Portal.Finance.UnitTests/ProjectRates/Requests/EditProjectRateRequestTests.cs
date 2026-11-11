using AwesomeAssertions;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.BusinessRules;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Encryptor;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Loaders;
using Modsen.Portal.Finance.Requests.ProjectRates.Requests;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.ProjectRates.Requests;

public class EditProjectRateRequestTests
{
    private readonly Mock<IIsProjectRateUniqueForDateBusinessRule> _isProjectRateUniqueForDateBusinessRule = new();
    private readonly Mock<IProjectUserLoader> _projectUserLoaderMock = new();
    private readonly Mock<ICanFinanceUserEditProjectRatesBusinessRule> _canFinanceUserEditProjectRateBusinessRuleMock = new();
    private readonly Mock<IProjectRateEncryptor> _projectRateEncryptorMock = new();
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly EditProjectRateRequest _editProjectRateRequest;

    public EditProjectRateRequestTests()
    {
        _editProjectRateRequest = new EditProjectRateRequest(
            _isProjectRateUniqueForDateBusinessRule.Object,
            _projectUserLoaderMock.Object,
            _canFinanceUserEditProjectRateBusinessRuleMock.Object,
            _projectRateEncryptorMock.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenRateNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        _dbContextMock.SetupGet(x => x.ProjectRates)
            .Returns(new List<DataAccess.Npgsql.Entities.ProjectRateEntity>().BuildMockDbSet().Object);

        // Act
        var action = () => _editProjectRateRequest.Handle(GetCommand(), _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.ProjectRateNotFound);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotBelongToProjectOnNewDateThrowInvalidDataAppException()
    {
        // Arrange
        var rate = TestDataGenerator.GetProjectRateEntity();
        var request = GetCommand(rateFrom: rate.RateFrom.AddMonths(1));

        _dbContextMock.SetupGet(x => x.ProjectRates)
            .Returns(new List<DataAccess.Npgsql.Entities.ProjectRateEntity> { rate }.BuildMockDbSet().Object);

        _projectUserLoaderMock
            .Setup(x => x.GetBillableUserProjectOnSpecificMonthAsync(rate.UserId, request.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var action = () => _editProjectRateRequest.Handle(request, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.UserDoesNotHaveAccessToSomeProjects);
    }

    [Fact]
    public async Task HandleWhenRateDateConflictsThrowEntityConflictException()
    {
        // Arrange
        var rate = TestDataGenerator.GetProjectRateEntity();
        var request = GetCommand(rateFrom: rate.RateFrom.AddMonths(1));

        _dbContextMock.SetupGet(x => x.ProjectRates)
            .Returns(new List<DataAccess.Npgsql.Entities.ProjectRateEntity> { rate }.BuildMockDbSet().Object);

        _projectUserLoaderMock
            .Setup(x => x.GetBillableUserProjectOnSpecificMonthAsync(rate.UserId, request.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync([rate.ProjectId]);

        _isProjectRateUniqueForDateBusinessRule
            .Setup(x => x.IsInvalid(rate.Id, rate.UserId, rate.ProjectId, request.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _canFinanceUserEditProjectRateBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(rate.UserId, rate.ProjectId, request.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var action = () => _editProjectRateRequest.Handle(request, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityConflictException>()
            .WithMessage(PortalMessages.ProjectRateDateConflicts);
    }

    [Fact]
    public async Task HandleWhenUserCannotEditRateThrowActionNotAllowedException()
    {
        // Arrange
        var request = GetCommand();
        var rate = TestDataGenerator.GetProjectRateEntity();

        _dbContextMock.SetupGet(x => x.ProjectRates)
            .Returns(new List<DataAccess.Npgsql.Entities.ProjectRateEntity> { rate }.BuildMockDbSet().Object);

        _projectUserLoaderMock
            .Setup(x => x.GetBillableUserProjectOnSpecificMonthAsync(rate.UserId, request.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync([rate.ProjectId]);

        _isProjectRateUniqueForDateBusinessRule
            .Setup(x => x.IsInvalid(rate.Id, rate.UserId, rate.ProjectId, request.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _canFinanceUserEditProjectRateBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(rate.UserId, rate.ProjectId, request.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var action = () => _editProjectRateRequest.Handle(request, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenRequestIsValidUpdateRateAndReturnDto()
    {
        // Arrange
        var request = GetCommand();

        var user = TestDataGenerator.GetFinanceUserEntity();
        var client = TestDataGenerator.GetFinanceClientEntity();
        var project = TestDataGenerator.GetFinanceProjectEntity(client);
        var rate = TestDataGenerator.GetProjectRateEntity(user, project);

        _dbContextMock.SetupGet(x => x.ProjectRates)
            .Returns(new List<DataAccess.Npgsql.Entities.ProjectRateEntity> { rate }.BuildMockDbSet().Object);

        _projectUserLoaderMock
            .Setup(x => x.GetBillableUserProjectOnSpecificMonthAsync(rate.UserId, request.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync([rate.ProjectId]);

        _isProjectRateUniqueForDateBusinessRule
            .Setup(x => x.IsInvalid(rate.Id, rate.UserId, rate.ProjectId, request.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _canFinanceUserEditProjectRateBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(rate.UserId, rate.ProjectId, request.RateFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _projectRateEncryptorMock
            .Setup(x => x.Encrypt(request.HourlyRate))
            .Returns("encrypted");

        _projectRateEncryptorMock
            .Setup(x => x.Decrypt("encrypted"))
            .Returns(request.HourlyRate);

        // Act
        var result = await _editProjectRateRequest.Handle(request, _cancellationToken);

        // Assert
        result.HourlyRate.Should().Be(request.HourlyRate);

        rate.HourlyRate.Should().Be("encrypted");
        rate.CurrencyId.Should().Be(request.CurrencyId);
        rate.RateFrom.Should().Be(request.RateFrom);

        _dbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once);
    }

    private EditProjectRateRequest.Command GetCommand(DateOnly? rateFrom = null)
    {
        return new EditProjectRateRequest.Command
        {
            RateId = 1,
            HourlyRate = 1,
            CurrencyId = CurrencyCodes.USDollar,
            RateFrom = rateFrom ?? new DateOnly(2025, 1, 1)
        };
    }
}
