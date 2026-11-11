using AwesomeAssertions;
using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.BusinessRules;
using Modsen.Portal.Identity.Contract;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.ProjectRates.BusinessRules;

public class CanFinanceUserEditProjectRatesBusinessRuleTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly CanFinanceUserEditProjectRatesBusinessRule _businessRule;

    public CanFinanceUserEditProjectRatesBusinessRuleTests()
    {
        _businessRule = new CanFinanceUserEditProjectRatesBusinessRule(
            _authenticatedUserContextMock.Object,
            _identityContractMock.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBussinesWhenUserHasManageAllRatesClaimReturnTrue()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.HasClaim(FinanceClaimTypeNames.ManageAllRates.Name))
            .Returns(true);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateOnly>(),
            _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBussinesWhenUserDoesntHaveManageRatesClaimReturnFalse()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.HasClaim(FinanceClaimTypeNames.ManageRates.Name))
            .Returns(false);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateOnly>(),
            _cancellationToken);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBussinesWhenInitiatorIsClientSalesManagerReturnTrue()
    {
        // Arrange
        var initiatorId = 1;
        var projectId = 2;

        BaseSetup(salesManagerId: initiatorId, projectId: projectId);

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId())
            .Returns(initiatorId);

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId())
            .Returns(initiatorId);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(It.IsAny<int>(), projectId, It.IsAny<DateOnly>(), _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBussinesWhenTargetUserIsSubordanateReturnTrue()
    {
        // Arrange
        var targetUserId = 1;
        var targetDate = new DateOnly(2026, 7, 15);
        var projectId = 1;

        BaseSetup(projectId: projectId, subordinates: [targetUserId]);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(targetUserId, projectId, targetDate,
            _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBussinesWhenSalesManagerIsSubordanateReturnTrue()
    {
        // Arrange
        var salesManagerId = 1;
        var projectId = 2;
        var targetDate = new DateOnly(2026, 7, 15);

        BaseSetup(salesManagerId: salesManagerId, subordinates: [salesManagerId], projectId: projectId);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(It.IsAny<int>(), projectId, targetDate,
            _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBussinesWhenUserIsNotSalesManagerAndNotTargetUserManagerAndNotManagerOfSalesManagerReturnFalse()
    {
        // Arrange
        var projectId = 1;
        BaseSetup(projectId: projectId);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(It.IsAny<int>(), projectId, It.IsAny<DateOnly>(),
            _cancellationToken);

        // Assert
        result.Should().BeFalse();
    }


    private void BaseSetup(int? salesManagerId = null, List<int>? subordinates = null, int? projectId = null)
    {
        var client = TestDataGenerator.GetFinanceClientEntity(salesManagerId: salesManagerId);

        _authenticatedUserContextMock.Setup(x => x.HasClaim(FinanceClaimTypeNames.ManageRates.Name))
            .Returns(true);

        _dbContextMock.SetupGet(x => x.FinanceProjects)
            .Returns(new List<FinanceProjectEntity> { TestDataGenerator.GetFinanceProjectEntity(client, id: projectId) }.BuildMockDbSet().Object);

        _identityContractMock.Setup(x => x.GetUserAndSubordinatesIdsAsync(It.IsAny<int>(), It.IsAny<int>(), _cancellationToken, false))
            .ReturnsAsync(subordinates ?? []);
    }
}
