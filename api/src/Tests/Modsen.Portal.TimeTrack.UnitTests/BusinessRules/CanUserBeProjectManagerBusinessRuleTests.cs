using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;

namespace Modsen.Portal.TimeTrack.UnitTests.BusinessRules;

public class CanUserBeProjectManagerBusinessRuleTests
{
    private readonly Mock<IIdentityContract> _identityContract = new();
    private readonly CanUserBeProjectManagerBusinessRule _businessRule;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CanUserBeProjectManagerBusinessRuleTests()
    {
        _businessRule = new CanUserBeProjectManagerBusinessRule(_identityContract.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserCanBeProjectManagerByDepartmentShouldReturnTrue()
    {
        // Arrange
        const int userId = 1;

        _identityContract.Setup(q => q.CanUserBeProjectManagerByDepartment(userId, _cancellationToken)).ReturnsAsync(true);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserCannotBeProjectManagerByDepartmentShouldReturnFalse()
    {
        // Arrange
        const int userId = 1;

        _identityContract.Setup(q => q.CanUserBeProjectManagerByDepartment(userId, _cancellationToken)).ReturnsAsync(false);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        // Assert
        result.Should().BeFalse();
    }
}
