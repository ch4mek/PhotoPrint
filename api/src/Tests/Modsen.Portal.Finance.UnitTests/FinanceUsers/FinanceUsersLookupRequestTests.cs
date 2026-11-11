using AwesomeAssertions;
using MockQueryable.Moq;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.Core.Builders;
using Modsen.Portal.Finance.Requests.FinanceUsers.Requests;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.FinanceUsers;

public class FinanceUsersLookupRequestTests
{
    private readonly Mock<IFinanceClientsAndUsersQueryBuilder> _managedClientsAndFinanceUsersQueryBuilderMock = new();
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly FinanceUsersLookupRequest _request;

    public FinanceUsersLookupRequestTests()
    {
        _request = new FinanceUsersLookupRequest(
            _managedClientsAndFinanceUsersQueryBuilderMock.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserAssignedOnPublicProjectsReturnsOnlyUsersOnPrivateProjects()
    {
        // Arrange
        var client = TestDataGenerator.GetFinanceClientEntity();
        var privateProject = TestDataGenerator.GetFinanceProjectEntity(client);
        var publicProject = TestDataGenerator.GetFinanceProjectEntity(client, isPublic: true);

        var user1 = TestDataGenerator.GetFinanceUserEntity(1);
        var user2 = TestDataGenerator.GetFinanceUserEntity(2);

        var projectUsers = new List<FinanceProjectUserEntity>
        {
            TestDataGenerator.GetFinanceProjectUserEntity(privateProject, user1, isTimeOnProjectBillable: true),
            TestDataGenerator.GetFinanceProjectUserEntity(publicProject, user2, isTimeOnProjectBillable: true)
        };

        _dbContextMock.SetupGet(x => x.FinanceProjectUsers).Returns(projectUsers.BuildMockDbSet().Object);
        _managedClientsAndFinanceUsersQueryBuilderMock
            .Setup(x => x.BuildAvailabletoViewFinanceProjectUsersAsync(
                It.IsAny<IQueryable<FinanceProjectUserEntity>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<FinanceProjectUserEntity> query, int _, int _, CancellationToken _) =>
                query.Where(x => !x.Project!.IsPublic && x.IsTimeOnProjectBillable));

        // Act
        var result = await _request.Handle(new FinanceUsersLookupRequest.Query(2026, 8), _cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(x => x.Id == user1.Id);
        result.Should().NotContain(x => x.Id == user2.Id);
    }
}
