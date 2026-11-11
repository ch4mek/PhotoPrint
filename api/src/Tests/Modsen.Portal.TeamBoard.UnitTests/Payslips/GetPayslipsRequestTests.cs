using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.OData;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Payslips.Requests;
using Modsen.Portal.TeamBoard.Requests.Payslips.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Payslips;

public class GetPayslipsRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _dbContextMock;
    private readonly Mock<IAuthenticatedUserContext> _userContextMock;
    private readonly GetPayslipsRequest _handler;
    private readonly CancellationToken _cancellationToken;

    public GetPayslipsRequestTests()
    {
        _dbContextMock = new Mock<ITeamBoardDbContext>();
        _userContextMock = new Mock<IAuthenticatedUserContext>();
        _cancellationToken = CancellationToken.None;

        _handler = new GetPayslipsRequest(_dbContextMock.Object, _userContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserRequestsOwnPayslipsReturnsPagedPayslips()
    {
        // Arrange
        const int userId = 1;
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "2",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpPayslip>(queryParameters);
        var request = new GetPayslipsRequest.Query(userId, queryOptions);

        var payslips = new List<Payslip>
        {
            new()
            {
                FileId = "file1",
                FileName = "payslip1.pdf",
                ReportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                UserId = userId,
                CreatedAtUtc = DateTime.UtcNow,
                User = new TeamBoardUser { UserId = userId,FirstName = "test",
                    LastName = "test",
                    Email = "test" }
            },
            new()
            {
                FileId = "file2",
                FileName = "payslip2.pdf",
                ReportDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                UserId = userId,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
                User = new TeamBoardUser { UserId = userId,FirstName = "test",
                    LastName = "test",
                    Email = "test" }
            },
            new()
            {
                FileId = "file3",
                FileName = "payslip3.pdf",
                ReportDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)),
                UserId = userId,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-2),
                User = new TeamBoardUser { UserId = userId,FirstName = "test",
                    LastName = "test",
                    Email = "test" }
            }
        };
        _userContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);
        _dbContextMock.Setup(x => x.Payslips)
            .Returns(payslips.BuildMockDbSet().Object);

        var expectedPayslips = payslips
            .Take(2)
            .Select(p => new OdpPayslip
            {
                FileId = p.FileId,
                FileName = p.FileName,
                ReportDate = p.ReportDate,
                UserId = p.UserId,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .ToList();

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Items.Should().BeEquivalentTo(expectedPayslips);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.Total.Should().Be(3);
    }

    [Fact]
    public async Task HandleWhenUserHasNoAccessThrowsActionNotAllowedException()
    {
        // Arrange
        const int userId = 1;
        const int otherUserId = 2;
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "2",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpPayslip>(queryParameters);
        var request = new GetPayslipsRequest.Query(userId, queryOptions);

        var payslips = new List<Payslip>
        {
            new()
            {
                FileId = "file1",
                FileName = "payslip1.pdf",
                ReportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                UserId = userId,
                CreatedAtUtc = DateTime.UtcNow,
                User = new TeamBoardUser { UserId = userId,FirstName = "test",
                    LastName = "test",
                    Email = "test" }
            }
        };
        _userContextMock.Setup(x => x.GetRequiredUserId()).Returns(otherUserId);
        _dbContextMock.Setup(x => x.Payslips)
            .Returns(payslips.BuildMockDbSet().Object);

        // Act
        var action = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage("You don't have access to another user's payslips");
    }

    [Fact]
    public async Task HandleWhenNoPayslipsExistReturnsEmptyPagedResult()
    {
        // Arrange
        const int userId = 1;
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "2",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpPayslip>(queryParameters);
        var request = new GetPayslipsRequest.Query(userId, queryOptions);

        var payslips = new List<Payslip>();
        _userContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);
        _dbContextMock.Setup(x => x.Payslips)
            .Returns(payslips.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Items.Should().BeEmpty();
        result.Page.Should().Be(0);
        result.PageSize.Should().Be(0);
        result.Total.Should().Be(0);
    }

    [Fact]
    public async Task HandleWhenPaginationSkipIsNonZeroReturnsCorrectPage()
    {
        // Arrange
        const int userId = 1;
        var now = DateTime.UtcNow;
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "1",
            [nameof(ODataRawQueryOptions.Skip)] = "1"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpPayslip>(queryParameters);
        var request = new GetPayslipsRequest.Query(userId, queryOptions);

        var payslips = new List<Payslip>
        {
            new()
            {
                FileId = "file1",
                FileName = "payslip1.pdf",
                ReportDate = DateOnly.FromDateTime(now),
                UserId = userId,
                CreatedAtUtc = now,
                User = new TeamBoardUser { UserId = userId,FirstName = "test",
                    LastName = "test",
                    Email = "test" }
            },
            new()
            {
                FileId = "file2",
                FileName = "payslip2.pdf",
                ReportDate = DateOnly.FromDateTime(now.AddDays(-1)),
                UserId = userId,
                CreatedAtUtc = now.AddDays(-1),
                User = new TeamBoardUser { UserId = userId,FirstName = "test",
                    LastName = "test",
                    Email = "test" }
            }
        };
        _userContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);
        _dbContextMock.Setup(x => x.Payslips)
            .Returns(payslips.BuildMockDbSet().Object);

        var expectedPayslip = new OdpPayslip
        {
            FileId = "file2",
            FileName = "payslip2.pdf",
            ReportDate = DateOnly.FromDateTime(now.AddDays(-1)),
            UserId = userId,
            CreatedAtUtc = now.AddDays(-1)
        };

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.Should().BeEquivalentTo([expectedPayslip]);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(1);
        result.Total.Should().Be(2);
    }

    [Fact]
    public async Task HandleWhenODataFilterAppliedReturnsFilteredPayslips()
    {
        // Arrange
        const int userId = 1;
        var now = DateTime.UtcNow;
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "2",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
            [nameof(ODataRawQueryOptions.Filter)] = "FileName eq 'payslip1.pdf'"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpPayslip>(queryParameters);
        var request = new GetPayslipsRequest.Query(userId, queryOptions);

        var payslips = new List<Payslip>
        {
            new()
            {
                FileId = "file1",
                FileName = "payslip1.pdf",
                ReportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                UserId = userId,
                CreatedAtUtc = now,
                User = new TeamBoardUser { UserId = userId,FirstName = "test",
                    LastName = "test",
                    Email = "test" }
            },
            new()
            {
                FileId = "file2",
                FileName = "payslip2.pdf",
                ReportDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                UserId = userId,
                CreatedAtUtc = now.AddDays(-1),
                User = new TeamBoardUser { UserId = userId,FirstName = "test",
                    LastName = "test",
                    Email = "test" }
            }
        };
        _userContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);
        _dbContextMock.Setup(x => x.Payslips)
            .Returns(payslips.BuildMockDbSet().Object);

        var expectedPayslip = new OdpPayslip
        {
            FileId = "file1",
            FileName = "payslip1.pdf",
            ReportDate = DateOnly.FromDateTime(now),
            UserId = userId,
            CreatedAtUtc = now
        };

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.Should().BeEquivalentTo([expectedPayslip]);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.Total.Should().Be(1);
    }

    [Fact]
    public async Task HandleWhenODataOrderByAppliedReturnsSortedPayslips()
    {
        // Arrange
        const int userId = 1;
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "2",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
            [nameof(ODataRawQueryOptions.OrderBy)] = "ReportDate desc"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpPayslip>(queryParameters);
        var request = new GetPayslipsRequest.Query(userId, queryOptions);

        var payslips = new List<Payslip>
        {
            new()
            {
                FileId = "file1",
                FileName = "payslip1.pdf",
                ReportDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                UserId = userId,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
                User = new TeamBoardUser { UserId = userId,FirstName = "test",
                    LastName = "test",
                    Email = "test" }
            },
            new()
            {
                FileId = "file2",
                FileName = "payslip2.pdf",
                ReportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                UserId = userId,
                CreatedAtUtc = DateTime.UtcNow,
                User = new TeamBoardUser { UserId = userId,FirstName = "test",
                    LastName = "test",
                    Email = "test" }
            }
        };
        _userContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);
        _dbContextMock.Setup(x => x.Payslips)
            .Returns(payslips.BuildMockDbSet().Object);

        var expectedPayslips = payslips
            .OrderByDescending(p => p.ReportDate)
            .Take(2)
            .Select(p => new OdpPayslip
            {
                FileId = p.FileId,
                FileName = p.FileName,
                ReportDate = p.ReportDate,
                UserId = p.UserId,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .ToList();

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Items.Should().BeEquivalentTo(expectedPayslips);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.Total.Should().Be(2);
    }
}
