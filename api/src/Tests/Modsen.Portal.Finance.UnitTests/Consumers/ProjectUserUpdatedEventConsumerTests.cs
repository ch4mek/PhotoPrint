using AwesomeAssertions;
using MassTransit;
using MockQueryable.Moq;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.Consumers;
using Modsen.Portal.TimeTrack.Contract.Events;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.Consumers;

public class ProjectUserUpdatedEventConsumerTests
{
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();
    private readonly Mock<IConnectionFactory> _connectionFactoryMock = new();


    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly ProjectUserUpdatedEventConsumer _consumer;

    public ProjectUserUpdatedEventConsumerTests()
    {
        _consumer = new ProjectUserUpdatedEventConsumer(_dbContextMock.Object, _connectionFactoryMock.Object);
    }

    [Fact]
    public async Task ConsumeWhenStartMovedLaterShouldDeleteRates()
    {
        // Arrange
        var dateFrom = new DateOnly(2026, 8, 3);
        var (model, context) = SetupProjectUser(dateFrom, null, dateFrom.AddMonths(1), null);

        var rates = new List<ProjectRateEntity>
        {
            TestDataGenerator.GetProjectRateEntity(1, model.userId, model.projectId, dateFrom),
            TestDataGenerator.GetProjectRateEntity(1, model.userId, model.projectId, dateFrom.AddMonths(1))
        };

        _dbContextMock.Setup(x => x.ProjectRates)
            .Returns(rates.BuildMockDbSet().Object);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        rates[0].IsDeleted.Should().BeTrue();
        rates[1].IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task ConsumeWhenEndMovedEarlierShouldDeleteRates()
    {
        // Arrange
        var dateFrom = new DateOnly(2026, 8, 3);
        var dateTo = new DateOnly(2026, 12, 3);
        var (model, context) = SetupProjectUser(dateFrom, dateTo, dateFrom, dateTo.AddMonths(-1));

        var rateNext = TestDataGenerator.GetProjectRateEntity(1, model.userId, model.projectId, dateTo);
        var ratePrev = TestDataGenerator.GetProjectRateEntity(1, model.userId, model.projectId,
            dateTo.AddMonths(-1), rateTo: rateNext.RateFrom.AddDays(-1));

        var rates = new List<ProjectRateEntity> { rateNext, ratePrev };

        _dbContextMock.Setup(x => x.ProjectRates)
            .Returns(rates.BuildMockDbSet().Object);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        rateNext.IsDeleted.Should().BeTrue();
        ratePrev.IsDeleted.Should().BeFalse();
        ratePrev.RateTo.Should().Be(null);
    }

    [Fact]
    public async Task ConsumeWhenEndChangedFromNullToDateShouldDeleteRates()
    {
        // Arrange
        var dateFrom = new DateOnly(2026, 8, 3);
        var newDateTo = new DateOnly(2026, 12, 3);
        var (model, context) = SetupProjectUser(dateFrom, null, dateFrom, newDateTo.AddMonths(-1));

        var rateNext = TestDataGenerator.GetProjectRateEntity(1, model.userId, model.projectId, newDateTo.AddMonths(1));
        var ratePrev = TestDataGenerator.GetProjectRateEntity(1, model.userId, model.projectId,
            newDateTo.AddMonths(-1), rateTo: rateNext.RateFrom.AddDays(-1));

        var rates = new List<ProjectRateEntity> { rateNext, ratePrev };

        _dbContextMock.Setup(x => x.ProjectRates)
            .Returns(rates.BuildMockDbSet().Object);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        rateNext.IsDeleted.Should().BeTrue();
        ratePrev.IsDeleted.Should().BeFalse();
        ratePrev.RateTo.Should().Be(null);
    }

    [Fact]
    public async Task ConsumeWhenEndChangedWithinMonthShouldNotDeleteRates()
    {
        // Arrange
        var dateFrom = new DateOnly(2026, 8, 3);
        var dateTo = new DateOnly(2026, 12, 3);
        var (model, context) = SetupProjectUser(dateFrom, dateTo, dateFrom, dateTo.AddDays(1));

        var rateNext = TestDataGenerator.GetProjectRateEntity(1, model.userId, model.projectId, dateTo.AddMonths(-1));
        var ratePrev = TestDataGenerator.GetProjectRateEntity(1, model.userId, model.projectId,
            dateFrom.AddMonths(1), rateTo: rateNext.RateFrom.AddDays(-1));

        var rates = new List<ProjectRateEntity> { rateNext, ratePrev };
 
        _dbContextMock.Setup(x => x.ProjectRates)
            .Returns(rates.BuildMockDbSet().Object);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        rateNext.IsDeleted.Should().BeFalse();
        ratePrev.IsDeleted.Should().BeFalse();

        _dbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once);
    }

    private (ProjectUserModel Model, Mock<ConsumeContext<ProjectUserUpdatedEvent>> Context) SetupProjectUser(
        DateOnly prevDateFrom, DateOnly? prevDateTo, DateOnly updDateFrom, DateOnly? updDateTo)
    {
        var project = TestDataGenerator.GetFinanceProjectEntity(TestDataGenerator.GetFinanceClientEntity());
        var user = TestDataGenerator.GetFinanceUserEntity();
        var projectUser = TestDataGenerator.GetFinanceProjectUserEntity(project, user, from: prevDateFrom, to: prevDateTo);
        _dbContextMock.Setup(x => x.FinanceProjectUsers)
            .Returns(new List<FinanceProjectUserEntity> { projectUser }.BuildMockDbSet().Object);

        var @event = TestDataGenerator.GetProjectUserUpdatedEvent(projectUser.Id, project.Id, user.Id, updDateFrom, updDateTo);
        var context = new Mock<ConsumeContext<ProjectUserUpdatedEvent>>();
        context.Setup(x => x.Message).Returns(@event);

        return (new ProjectUserModel(projectUser.Id, project.Id, user.Id), context);
    }

    private sealed record ProjectUserModel(int projectUserId, int userId, int projectId);
}
