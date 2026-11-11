using AwesomeAssertions;
using MockQueryable.Moq;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.ProjectRates.BusinessRules;

public class IsProjectRateFromDateValidBusinessRuleTests
{
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly IsProjectRateUniqueForDateBusinessRule _businessRule;

    public IsProjectRateFromDateValidBusinessRuleTests()
    {
        _businessRule = new IsProjectRateUniqueForDateBusinessRule(_dbContextMock.Object);
    }

    [Fact]
    public async Task IsInvalidWhenUserDoesNotHaveRateWithSameDateReturnFalse()
    {
        // Arrange
        var date = new DateOnly(2025, 1, 1);
        var rates = new List<DataAccess.Npgsql.Entities.ProjectRateEntity>
        {
            TestDataGenerator.GetProjectRateEntity(id: 1, userId: 1, projectId: 1, rateFrom: new DateOnly(2025, 2, 1))
        };

        _dbContextMock.SetupGet(x => x.ProjectRates)
            .Returns(rates.BuildMockDbSet().Object);

        // Act
        var result = await _businessRule.IsInvalid(
            rateId: null,
            userId: 1,
            projectId: 1,
            date,
            _cancellationToken);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsInvalidWhenUserAlreadyHasAnotherRateWithSameDateReturnTrue()
    {
        // Arrange
        var date = new DateOnly(2025, 1, 1);
        var rates = new List<DataAccess.Npgsql.Entities.ProjectRateEntity>
        {
            TestDataGenerator.GetProjectRateEntity(id: 1, userId: 1, projectId: 1, rateFrom: date)
        };

        _dbContextMock.SetupGet(x => x.ProjectRates)
            .Returns(rates.BuildMockDbSet().Object);

        // Act
        var result = await _businessRule.IsInvalid(
            rateId: null,
            userId: 1,
            projectId: 1,
            date,
            _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsInvalidWhenRateWithSameDateExistsInAnotherProjectReturnFalse()
    {
        // Arrange
        var date = new DateOnly(2025, 1, 1);
        var rates = new List<DataAccess.Npgsql.Entities.ProjectRateEntity>
        {
            TestDataGenerator.GetProjectRateEntity(id: 1, userId: 1, projectId: 2, rateFrom: date)
        };

        _dbContextMock.SetupGet(x => x.ProjectRates)
            .Returns(rates.BuildMockDbSet().Object);

        // Act
        var result = await _businessRule.IsInvalid(
            rateId: null,
            userId: 1,
            projectId: 1,
            date,
            _cancellationToken);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsInvalidWhenSameRateIsBeingUpdatedReturnFalse()
    {
        // Arrange
        var rateId = 1;
        var date = new DateOnly(2025, 1, 1);
        var rates = new List<DataAccess.Npgsql.Entities.ProjectRateEntity>
        {
            TestDataGenerator.GetProjectRateEntity(id: rateId, userId: 1, projectId: 1, rateFrom: date)
        };

        _dbContextMock.SetupGet(x => x.ProjectRates)
            .Returns(rates.BuildMockDbSet().Object);

        // Act
        var result = await _businessRule.IsInvalid(
            rateId,
            userId: 1,
            projectId: 1,
            date,
            _cancellationToken);

        // Assert
        result.Should().BeFalse();
    }
}
