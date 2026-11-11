using AwesomeAssertions;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.Core.Builders;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Encryptor;
using Modsen.Portal.Finance.Requests.ProjectRates.Requests;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TeamBoard.Contract;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.ProjectRates.Requests;

public class GetAllProjectRatesRequestTests
{
    private readonly Mock<IProjectRateEncryptor> _numberEncryptorMock = new();
    private readonly Mock<IGetProjectRatesQueryBuilder> _getRatesQueryBuilderMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();
    private readonly Mock<ITeamBoardContract> _teamBoardContractMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetAllProjectRatesRequest _request;

    public GetAllProjectRatesRequestTests()
    {
        _request = new GetAllProjectRatesRequest(
            _getRatesQueryBuilderMock.Object,
            _numberEncryptorMock.Object,
            _identityContractMock.Object,
            _teamBoardContractMock.Object);

        SetupBuilder();
    }

    private void SetupBuilder()
    {
        _getRatesQueryBuilderMock
            .Setup(x => x.ForYearAndMonth(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(_getRatesQueryBuilderMock.Object);

        _getRatesQueryBuilderMock
            .Setup(x => x.WithPagination(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(_getRatesQueryBuilderMock.Object);

        _getRatesQueryBuilderMock
            .Setup(x => x.WithFilter(It.IsAny<RatesManagementBaseFilterCoreDto>()))
            .Returns(_getRatesQueryBuilderMock.Object);

        _getRatesQueryBuilderMock
            .Setup(x => x.WithOrdering(It.IsAny<FinanceOrderingCoreDto>()))
            .Returns(_getRatesQueryBuilderMock.Object);
    }

    [Fact]
    public async Task HandleWhenNoUsersShouldReturnEmptyPage()
    {
        var dto = CreateDto([], []);
        var query = GetQuery();

        BaseSetup(query, dto, It.IsAny<int>());

        // Act
        var result = await _request.Handle(query, _cancellationToken);

        // Assert
        result.Items.Should().BeEmpty();
        result.Total.Should().Be(0);
        result.Page.Should().Be(query.PageNumber);
        result.PageSize.Should().Be(query.PageSize);
    }

    [Fact]
    public async Task HandleWhenHistoricalDepartmentExistsShouldUseHistoricalDepartment()
    {
        // Arrange
        var query = GetQuery();

        var historicalDepId = "Historical";

        (var user, _, var dto) = GetUserProjectAndDto();

        _getRatesQueryBuilderMock
            .Setup(x => x.BuildRates(_cancellationToken, false))
            .ReturnsAsync(dto);

        _identityContractMock
            .Setup(x => x.UsersHistoricalDepartment(query.Year, query.Month, new List<int> { user.Id }, _cancellationToken))
            .ReturnsAsync(new Dictionary<int, string> { { user.Id, historicalDepId } });

        _teamBoardContractMock
            .Setup(x => x.CanUsersBeRetrievedByUser(It.IsAny<int[]>(), It.IsAny<AccessOptions>(), _cancellationToken))
            .ReturnsAsync(new Dictionary<int, bool> { { user.Id, true } });

        // Act
        var result = await _request.Handle(query, _cancellationToken);

        // Assert
        result.Items.Single().User.DepartmentId.Should().Be(historicalDepId);
    }

    [Fact]
    public async Task HandleWhenHistoricalDepartmentMissingShouldUseCurrentDepartment()
    {
        // Arrange
        var query = GetQuery();

        (var user, _, var dto) = GetUserProjectAndDto();

        BaseSetup(query, dto, user.Id);

        // Act
        var result = await _request.Handle(query, _cancellationToken);

        // Assert
        result.Items.Single().User.DepartmentId.Should().Be(user.DepartmentId);
    }

    [Fact]
    public async Task Handle_ShouldDecryptHourlyRate()
    {
        // Arrange
        var query = GetQuery();
        var decryptedRate = 125m;

        (var user, var project, var dto) = GetUserProjectAndDto();

        BaseSetup(query, dto, user.Id);

        _numberEncryptorMock
            .Setup(x => x.Decrypt("encrypted"))
            .Returns(decryptedRate);

        // Act
        var result = await _request.Handle(query, _cancellationToken);

        // Assert
        result.Items.Single().ProjectRates.Single().HourlyRate.Should().Be(decryptedRate);

        _numberEncryptorMock.Verify(x => x.Decrypt("encrypted"), Times.Once);
    }

    [Fact]
    public async Task HandleWhenCurrenciesEqualShouldCalculateTotalRate()
    {
        // Arrange
        var query = GetQuery();
        var decryptedRate1 = 100m;
        var decryptedRate2 = 250m;

        var user = TestDataGenerator.GetFinanceUserEntity();
        var project1 = TestDataGenerator.GetFinanceProjectEntity(TestDataGenerator.GetFinanceClientEntity(), id: 1);
        var project2 = TestDataGenerator.GetFinanceProjectEntity(TestDataGenerator.GetFinanceClientEntity(), id: 2);
        var rate1 = TestDataGenerator.GetProjectRateEntity(user, project1, "encrypted1");
        var rate2 = TestDataGenerator.GetProjectRateEntity(user, project2, "encrypted2");

        var dto = CreateDto(
            [
                TestDataGenerator.GetFinanceProjectUserEntity(project1, user),
                TestDataGenerator.GetFinanceProjectUserEntity(project2, user)
            ],
            [ rate1, rate2 ]);

        BaseSetup(query, dto, user.Id);

        _numberEncryptorMock.Setup(x => x.Decrypt(rate1.HourlyRate)).Returns(decryptedRate1);
        _numberEncryptorMock.Setup(x => x.Decrypt(rate2.HourlyRate)).Returns(decryptedRate2);

        // Act
        var result = await _request.Handle(query, _cancellationToken);

        // Assert
        var item = result.Items.Single();

        item.TotalRate.Should().Be(decryptedRate1 + decryptedRate2);
        item.CurrencyId.Should().Be(rate1.CurrencyId);
    }

    [Fact]
    public async Task HandleWhenCurrenciesDifferentShouldNotCalculateTotalRate()
    {
        // Arrange
        var query = GetQuery();

        var user = TestDataGenerator.GetFinanceUserEntity();
        var project1 = TestDataGenerator.GetFinanceProjectEntity(TestDataGenerator.GetFinanceClientEntity(), id: 1);
        var project2 = TestDataGenerator.GetFinanceProjectEntity(TestDataGenerator.GetFinanceClientEntity(), id: 2);
        var rate1 = TestDataGenerator.GetProjectRateEntity(user, project1, "encrypted1", currency: CurrencyCodes.USDollar);
        var rate2 = TestDataGenerator.GetProjectRateEntity(user, project2, "encrypted2", currency: CurrencyCodes.BelarusianRuble);

        var dto = CreateDto(
            [
                TestDataGenerator.GetFinanceProjectUserEntity(project1, user),
                TestDataGenerator.GetFinanceProjectUserEntity(project2, user)
            ],
            [rate1, rate2]);

        BaseSetup(query, dto, user.Id);

        // Act
        var result = await _request.Handle(query, _cancellationToken);

        // Assert
        var item = result.Items.Single();

        item.TotalRate.Should().BeNull();
        item.CurrencyId.Should().BeNull();
    }

    [Fact]
    public async Task HandleShouldMapVisibility()
    {
        // Arrange
        var query = GetQuery();

        (var user, var project, var dto) = GetUserProjectAndDto();


        _getRatesQueryBuilderMock
            .Setup(x => x.BuildRates(_cancellationToken, false))
            .ReturnsAsync(dto);

        _identityContractMock
            .Setup(x => x.UsersHistoricalDepartment(query.Year, query.Month, new List<int> { user.Id }, _cancellationToken))
            .ReturnsAsync(new Dictionary<int, string>());

        _teamBoardContractMock
            .Setup(x => x.CanUsersBeRetrievedByUser(It.IsAny<int[]>(), It.IsAny<AccessOptions>(), _cancellationToken))
            .ReturnsAsync(new Dictionary<int, bool> { { user.Id, false } });

        // Act
        var result = await _request.Handle(query, _cancellationToken);

        // Assert
        result.Items.Single().CanUserInfoBeRetrievedByCurrentUser.Should().BeFalse();
    }

    private static GetAllProjectRatesRequest.Query GetQuery()
    {
        return new()
        {
            Year = 2026,
            Month = 6,
            PageNumber = 0,
            PageSize = 25,
            Filter = new RatesFilterCoreDto(),
            Ordering = new FinanceOrderingCoreDto()
        };
    }

    private (FinanceUserEntity User, FinanceProjectEntity Project, GetProjectRatesQueryBuilderDto Dto) GetUserProjectAndDto()
    {
        var user = TestDataGenerator.GetFinanceUserEntity();
        var project = TestDataGenerator.GetFinanceProjectEntity(TestDataGenerator.GetFinanceClientEntity());

        var dto = CreateDto(
            [TestDataGenerator.GetFinanceProjectUserEntity(project, user)],
            [TestDataGenerator.GetProjectRateEntity(user, project, "encrypted")]);

        return (user, project, dto);

    }

    private void BaseSetup(GetAllProjectRatesRequest.Query query, GetProjectRatesQueryBuilderDto dto, int userId)
    {
        _getRatesQueryBuilderMock
            .Setup(x => x.BuildRates(_cancellationToken, false))
            .ReturnsAsync(dto);

        _identityContractMock
            .Setup(x => x.UsersHistoricalDepartment(query.Year, query.Month, new List<int> { userId }, _cancellationToken))
            .ReturnsAsync(new Dictionary<int, string>());

        _teamBoardContractMock
            .Setup(x => x.CanUsersBeRetrievedByUser(It.IsAny<int[]>(), It.IsAny<AccessOptions>(), _cancellationToken))
            .ReturnsAsync(new Dictionary<int, bool> { { userId, true } });
    }

    private static GetProjectRatesQueryBuilderDto CreateDto(
        IEnumerable<FinanceProjectUserEntity> assigns,
        IEnumerable<DataAccess.Npgsql.Entities.ProjectRateEntity> rates,
        HashSet<int>? managed = null)
    {
        return new GetProjectRatesQueryBuilderDto
        {
            UsersIds = assigns
                .Select(x => x.UserId)
                .Distinct()
                .ToList(),

            ManagedProjectUsersIds = managed ?? new HashSet<int>(),
            TotalUsersCount = assigns
                .Select(x => x.UserId)
                .Distinct()
                .Count(),

            ProjectUsers = assigns
                .GroupBy(x => x.UserId)
                //.Select(g => new TestGrouping<int, FinanceProjectUserEntity>(g.Key, g))
                //.Cast<IGrouping<int, FinanceProjectUserEntity>>()
                .ToList(),

            Rates = rates
                .GroupBy(x => x.UserId)
                //.Select(g => new TestGrouping<int, ProjectRateEntity>(g.Key, g))
                //.Cast<IGrouping<int, ProjectRateEntity>>()
                .ToList()
        };
    }
}
