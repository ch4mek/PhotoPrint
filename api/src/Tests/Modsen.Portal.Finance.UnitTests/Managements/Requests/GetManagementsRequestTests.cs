using AwesomeAssertions;
using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.Core.Builders;
using Modsen.Portal.Finance.Requests.Core.Converters;
using Modsen.Portal.Finance.Requests.Expenses.Core.Encryptor;
using Modsen.Portal.Finance.Requests.Managements.Core.Calculators;
using Modsen.Portal.Finance.Requests.Managements.Requests;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Encryptor;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TeamBoard.Contract.Contract.Input;
using Modsen.Portal.TimeTrack.Contract;
using Modsen.Portal.TimeTrack.Contract.Contract.Input;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.Managements.Requests;

public sealed class GetManagementsRequestTests
{
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();
    private readonly Mock<IManagementsCalculator> _managementsCalculatorMock = new();
    private readonly Mock<ITimeTrackContract> _timeTrackContractMock = new();
    private readonly Mock<ITeamBoardContract> _teamBoardContractMock = new();
    private readonly Mock<ICurrencyConverter> _currencyConverterMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();
    private readonly Mock<IExpenseEncryptor> _expenseEncryptorMock = new();
    private readonly Mock<IProjectRateEncryptor> _numberEncryptorMock = new();
    private readonly Mock<IGetProjectRatesQueryBuilder> _getProjectRatesQueryBuilderMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetManagementsRequest _handler;

    public GetManagementsRequestTests()
    {
        _handler = new GetManagementsRequest(
            _dbContextMock.Object,
            _managementsCalculatorMock.Object,
            _timeTrackContractMock.Object,
            _teamBoardContractMock.Object,
            _currencyConverterMock.Object,
            _identityContractMock.Object,
            _expenseEncryptorMock.Object,
            _numberEncryptorMock.Object,
            _getProjectRatesQueryBuilderMock.Object);
    }

    [Fact]
    public async Task HandleWhenBynCurrencyDoesNotExistShouldThrowAppException()
    {
        // Arrange
        var query = new GetManagementsRequest.Query
        {
            Year = 2026,
            Month = 7,
            PageNumber = 1,
            PageSize = 10
        };

        _dbContextMock.Setup(x => x.Currencies)
            .Returns(new List<CurrencyEntity>().BuildMockDbSet().Object);

        // Act
        var act = async () => await _handler.Handle(query, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<AppException>()
            .WithMessage("Byn currency does not exist in the database.");
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldReturnPageableManagementsDto()
    {
        // Arrange
        var query = new GetManagementsRequest.Query
        {
            Year = 2026,
            Month = 7,
            PageNumber = 1,
            PageSize = 10,
        };

        var currencies = new List<CurrencyEntity>
        {
            TestDataGenerator.GetCurrencyEntity(CurrencyCodes.BelarusianRuble)
        };

        _getProjectRatesQueryBuilderMock
            .Setup(x => x.WithOrdering(It.IsAny<FinanceOrderingCoreDto>()))
            .Returns(_getProjectRatesQueryBuilderMock.Object);

        _getProjectRatesQueryBuilderMock
            .Setup(x => x.BuildRates(It.IsAny<CancellationToken>(), It.IsAny<bool>()))
            .ReturnsAsync(new GetProjectRatesQueryBuilderDto
            {
                UsersIds = [],
                ManagedProjectUsersIds = [],
                TotalUsersCount = 0,
                ProjectUsers = [],
                Rates = []
            });

        _dbContextMock.Setup(x => x.Currencies)
            .Returns(currencies.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.ProjectRates)
            .Returns(new List<DataAccess.Npgsql.Entities.ProjectRateEntity>().BuildMockDbSet().Object);

        _timeTrackContractMock
            .Setup(x => x.GetTimesheetByUsers(It.IsAny<IReadOnlyCollection<TimesheetByUserInput>>(), _cancellationToken))
            .ReturnsAsync([]);

        _teamBoardContractMock
            .Setup(x => x.GetUsersSalariesByMonth(It.IsAny<UsersSalaryByMonthInput>(), _cancellationToken))
            .ReturnsAsync([]);

        _identityContractMock
            .Setup(x => x.UsersHistoricalDepartment(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<int>>(), _cancellationToken))
            .ReturnsAsync(new Dictionary<int, string>());

        _managementsCalculatorMock
            .Setup(x => x.Calculate(It.IsAny<ManagementsCalculator.Request>(), _cancellationToken))
            .ReturnsAsync(new ManagementsCalculator.Response());

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Year.Should().Be(2026);
        result.Month.Should().Be(7);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task HandleWhenShouldCalculateMainStatisticsIsTrueShouldCalculateMainStatistics()
    {
        // Arrange
        var query = new GetManagementsRequest.Query
        {
            Year = 2026,
            Month = 6,
            PageNumber = 1,
            PageSize = 10,
            ShouldCalculateMainStatistics = true
        };

        var currencies = new List<CurrencyEntity>
        {
            TestDataGenerator.GetCurrencyEntity(CurrencyCodes.BelarusianRuble)
        };

        var expenses = new List<ExpenseEntity>
        {
            TestDataGenerator.GetExpenseEntity()
        };

        _getProjectRatesQueryBuilderMock
           .Setup(x => x.BuildRates(It.IsAny<CancellationToken>(), It.IsAny<bool>()))
           .ReturnsAsync(new GetProjectRatesQueryBuilderDto
           {
               UsersIds = [],
               ManagedProjectUsersIds = [],
               TotalUsersCount = 0,
               ProjectUsers = [],
               Rates = []
           });

        _identityContractMock
            .Setup(x => x.UsersHistoricalDepartment(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<int>>(), _cancellationToken))
            .ReturnsAsync(new Dictionary<int, string>());

        _dbContextMock.Setup(x => x.Currencies)
            .Returns(currencies.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.ProjectRates)
            .Returns(new List<DataAccess.Npgsql.Entities.ProjectRateEntity>().BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.Expenses)
            .Returns(expenses.BuildMockDbSet().Object);

        _timeTrackContractMock
            .Setup(x => x.GetTimesheetByUsers(It.IsAny<IReadOnlyCollection<TimesheetByUserInput>>(), _cancellationToken))
            .ReturnsAsync([]);

        _teamBoardContractMock
            .Setup(x => x.GetUsersSalariesByMonth(It.IsAny<UsersSalaryByMonthInput>(), _cancellationToken))
            .ReturnsAsync([]);

        _managementsCalculatorMock
            .Setup(x => x.Calculate(It.IsAny<ManagementsCalculator.Request>(), _cancellationToken))
            .ReturnsAsync(new ManagementsCalculator.Response());

        _expenseEncryptorMock
            .Setup(x => x.Decrypt(It.IsAny<string?>()))
            .Returns(20m);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Expenses.Should().Be(20m);
        result.Profit.Should().BeNull();

        result.IncomeBynAmount.Should().BeNull();
        result.SalaryAmount.Should().BeNull();
        result.AverageKpi.Should().BeNull();

        _managementsCalculatorMock.Verify(x => x.Calculate(
            It.Is<ManagementsCalculator.Request>(r => r.ShouldCalculateProfit == true),
            _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenShouldCalculateColumnStatisticsIsTrueShouldCalculateColumnStatistics()
    {
        // Arrange
        var query = new GetManagementsRequest.Query
        {
            Year = 2026,
            Month = 6,
            PageNumber = 1,
            PageSize = 10,
            ShouldCalculateColumnStatistics = true
        };

        var currencies = new List<CurrencyEntity>
        {
            TestDataGenerator.GetCurrencyEntity(CurrencyCodes.BelarusianRuble)
        };

        _getProjectRatesQueryBuilderMock
            .Setup(x => x.BuildRates(It.IsAny<CancellationToken>(), It.IsAny<bool>()))
            .ReturnsAsync(new GetProjectRatesQueryBuilderDto
            {
                UsersIds = [],
                ManagedProjectUsersIds = [],
                TotalUsersCount = 0,
                ProjectUsers = [],
                Rates = []
            });

        _identityContractMock
            .Setup(x => x.UsersHistoricalDepartment(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<int>>(), _cancellationToken))
            .ReturnsAsync(new Dictionary<int, string>());

        _dbContextMock.Setup(x => x.Currencies)
            .Returns(currencies.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.ProjectRates)
            .Returns(new List<DataAccess.Npgsql.Entities.ProjectRateEntity>().BuildMockDbSet().Object);

        _timeTrackContractMock
            .Setup(x => x.GetTimesheetByUsers(It.IsAny<IReadOnlyCollection<TimesheetByUserInput>>(), _cancellationToken))
            .ReturnsAsync([]);

        _teamBoardContractMock
            .Setup(x => x.GetUsersSalariesByMonth(It.IsAny<UsersSalaryByMonthInput>(), _cancellationToken))
            .ReturnsAsync([]);

        _managementsCalculatorMock
            .Setup(x => x.Calculate(It.IsAny<ManagementsCalculator.Request>(), _cancellationToken))
            .ReturnsAsync(new ManagementsCalculator.Response());

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.IncomeBynAmount.Should().BeNull();
        result.SalaryAmount.Should().BeNull();

        result.Expenses.Should().BeNull();
        result.Profit.Should().BeNull();

        _managementsCalculatorMock.Verify(x => x.Calculate(
            It.Is<ManagementsCalculator.Request>(r => r.ShouldCalculateColumnStatistics == true),
            _cancellationToken),
            Times.Once);
    }
}
