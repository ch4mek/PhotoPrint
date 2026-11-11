using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Users.Core;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Core;

public class UserDepartmentHistoryComposerTests
{
    private readonly Mock<IClockService> _clockMock;
    private readonly UserDepartmentHistoryComposer _composer;

    private readonly DateOnly _today = new DateOnly(2025, 1, 10);

    public UserDepartmentHistoryComposerTests()
    {
        _clockMock = new Mock<IClockService>();
        _clockMock.Setup(c => c.TodayUtc()).Returns(_today);

        _composer = new UserDepartmentHistoryComposer(_clockMock.Object);
    }

    [Fact]
    public void ComposeShouldAddNewDepartmentWhenNoCurrentDepartment()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Test",
            LastName = "Test",
        };

        // Act
        _composer.Compose("Net", user);

        // Assert
        var history = user.UserDepartmentHistory.Single();

        Assert.Equal("Net", history.DepartmentId);
        Assert.Equal(_today, history.DateFrom);
        Assert.Null(history.DateTo);
    }

    [Fact]
    public void ComposeShouldDoNothingWhenSameDepartment()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Test",
            LastName = "Test",
            UserDepartmentHistory =
            {
                new UserDepartmentHistory
                {
                    DepartmentId = "Net",
                    DateFrom = _today,
                    DateTo = null
                }
            }
        };

        // Act
        _composer.Compose("Net", user);

        // Assert
        Assert.Single(user.UserDepartmentHistory);
        Assert.Null(user.UserDepartmentHistory.First().DateTo);
    }

    [Fact]
    public void ComposeShouldClosePreviousAndAddNewWhenDepartmentHistory()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Test",
            LastName = "Test",
            UserDepartmentHistory =
            {
                new UserDepartmentHistory
                {
                    DepartmentId = "Net",
                    DateFrom = _today.AddDays(-10),
                    DateTo = null
                }
            }
        };

        // Act
        _composer.Compose("Js", user);

        // Assert
        Assert.Equal(2, user.UserDepartmentHistory.Count);

        var oldDep = user.UserDepartmentHistory.First(x => x.DepartmentId == "Net");
        var newDep = user.UserDepartmentHistory.First(x => x.DepartmentId == "Js");

        Assert.Equal(_today.AddDays(-1), oldDep.DateTo);

        Assert.Equal("Js", newDep.DepartmentId);
        Assert.Equal(_today, newDep.DateFrom);
        Assert.Null(newDep.DateTo);
    }

    [Fact]
    public void ComposeShouldDoNothingWhenDepartmentIdIsNull()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Test",
            LastName = "Test",
        };

        // Act
        _composer.Compose(null, user);

        // Assert
        Assert.Empty(user.UserDepartmentHistory);
    }

    [Fact]
    public void ComposeShouldIgnorePastDepartmentsAndAddNew()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Test",
            LastName = "Test",
            UserDepartmentHistory =
            {
                new UserDepartmentHistory
                {
                    DepartmentId = "Net",
                    DateFrom = _today.AddDays(-20),
                    DateTo = _today.AddDays(-10)
                }
            }
        };

        // Act
        _composer.Compose("Js", user);

        // Assert
        Assert.Equal(2, user.UserDepartmentHistory.Count);

        var newDep = user.UserDepartmentHistory.Last();
        Assert.Equal("Js", newDep.DepartmentId);
        Assert.Null(newDep.DateTo);
    }
}
