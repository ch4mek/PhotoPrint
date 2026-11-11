using MockQueryable.Moq;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class GetUsersActivePeriodsInDepartmentsRequestTests
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly GetUsersActivePeriodsInDepartmentsRequest _handler;

    public GetUsersActivePeriodsInDepartmentsRequestTests()
    {
        _handler = new GetUsersActivePeriodsInDepartmentsRequest(
            _identityDbContextMock.Object);
    }

    [Fact]
    public async Task HandleShouldReturnNullWhenDepartmentIdsIsNull()
    {
        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>().BuildMockDbSet().Object);
        var query = new GetUsersActivePeriodsInDepartmentsRequest.Query([1], null);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task HandleShouldReturnNullWhenDepartmentIdsIsEmpty()
    {
        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>().BuildMockDbSet().Object);
        var query = new GetUsersActivePeriodsInDepartmentsRequest.Query([1], Array.Empty<string>());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task HandleShouldReturnEmptyRangesWhenNoMatchingDepartments()
    {
        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>
            {
                new() {
                    FirstName = "Test",
                    LastName = "Test",
                    Id = 1,
                    UserDepartmentHistory =
                    {
                        new UserDepartmentHistory
                        {
                            DepartmentId = "Net",
                            DateFrom = new DateOnly(2024, 1, 1),
                            DateTo = null
                        }
                    }
                } }.BuildMockDbSet().Object);

        var query = new GetUsersActivePeriodsInDepartmentsRequest.Query([1], ["Js"]);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result[1]);
    }

    [Fact]
    public async Task HandleShouldReturnCorrectDateRanges()
    {
        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>
            {
                new() {
                    FirstName = "Test",
                    LastName = "Test",
                    Id = 1,
                    UserDepartmentHistory =
                    {
                        new UserDepartmentHistory
                        {
                            DepartmentId = "Net",
                            DateFrom = new DateOnly(2024, 1, 1),
                            DateTo = new DateOnly(2024, 1, 10)
                        },
                        new UserDepartmentHistory
                        {
                            DepartmentId = "Js",
                            DateFrom = new DateOnly(2024, 2, 1),
                            DateTo = null
                        }
                    }
                } }.BuildMockDbSet().Object);

        var query = new GetUsersActivePeriodsInDepartmentsRequest.Query([1], ["Net", "Js"]);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result[1].Length);

        var range1 = result[1][0];
        var range2 = result[1][1];

        Assert.Equal(new DateOnly(2024, 1, 1), range1.Start);
        Assert.Equal(new DateOnly(2024, 1, 10), range1.End);

        Assert.Equal(new DateOnly(2024, 2, 1), range2.Start);
        Assert.Equal(DateOnly.MaxValue, range2.End);
    }

    [Fact]
    public async Task HandleShouldIgnoreInvalidRangesWhenDateFromGreaterThanDateTo()
    {
        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>
            {
                new() {
                    FirstName = "Test",
                    LastName = "Test",
                    Id = 1,
                    UserDepartmentHistory =
                    {
                        new UserDepartmentHistory
                        {
                            DepartmentId = "Net",
                            DateFrom = new DateOnly(2024, 2, 1),
                            DateTo = new DateOnly(2024, 1, 1)
                        }
                    }                    
                } }.BuildMockDbSet().Object);

        var query = new GetUsersActivePeriodsInDepartmentsRequest.Query([1], ["Net"]);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result[1]);
    }

    [Fact]
    public async Task HandleShouldReturnOnlyRequestedUsers()
    {
        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>
            {
                new() {
                    FirstName = "Test",
                    LastName = "Test",
                    Id = 1,
                },
                new() {
                    FirstName = "Test",
                    LastName = "Test",
                    Id = 2
                } }.BuildMockDbSet().Object);

        var query = new GetUsersActivePeriodsInDepartmentsRequest.Query([2], ["Net"]);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.True(result.ContainsKey(2));
    }
}
