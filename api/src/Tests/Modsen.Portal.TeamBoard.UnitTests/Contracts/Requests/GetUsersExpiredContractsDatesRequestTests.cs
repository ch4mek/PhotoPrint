using MockQueryable.Moq;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Contracts.Requests;

public class GetUsersExpiredContractsDatesRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly GetUsersExpiredContractsDatesRequest _handler;

    public GetUsersExpiredContractsDatesRequestTests()
    {
        _handler = new GetUsersExpiredContractsDatesRequest(_dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenNoContractsReturnsEmptyList()
    {
        // Arrange
        _dbContextMock
            .Setup(c => c.UserContracts)
            .Returns(new List<UserContract>().BuildMockDbSet().Object);

        var request = new GetUsersExpiredContractsDatesRequest.Query(new DateOnly(2025, 1, 1));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleOnlyInactiveContractsIgnored()
    {
        // Arrange
        var data = new List<UserContract>
        {
            new()
            {
                UserId = 1,
                IsActive = false,
                ContractFrom = new DateOnly(2023,1,1),
                ContractTo = new DateOnly(2023,12,31),
                FireDate = null,
                User = new TeamBoardUser
                {
                    HireDate = new DateOnly(2023,1,1),
                    UserId = 1,
                    FirstName = "test",
                    LastName = "test",
                    Email = "test",
                }
            }
        };

        _dbContextMock
            .Setup(c => c.UserContracts)
            .Returns(data.BuildMockDbSet().Object);

        var request = new GetUsersExpiredContractsDatesRequest.Query(new DateOnly(2023, 12, 31));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task HandlContractEndMatchesDateIncludesResult()
    {
        // Arrange
        var targetDate = new DateOnly(2024, 2, 28);

        var data = new List<UserContract>
        {
            new UserContract
            {
                UserId = 1,
                IsActive = true,
                ContractFrom = new DateOnly(2023,1,1),
                ContractTo = targetDate,
                FireDate = null,
                User = new TeamBoardUser
                {
                    HireDate = new DateOnly(2023,1,1),
                    UserId = 1,
                    FirstName = "test",
                    LastName = "test",
                    Email = "test",
                }
            },
            new UserContract
            {
                UserId = 2,
                IsActive = true,
                ContractFrom = new DateOnly(2023,1,1),
                ContractTo = new DateOnly(2025,1,1),
                FireDate = null,
                User = new TeamBoardUser
                {
                    HireDate = new DateOnly(2023,1,1),
                    UserId = 2,
                    FirstName = "test",
                    LastName = "test",
                    Email = "test",
                }
            }
        };

        _dbContextMock
            .Setup(c => c.UserContracts)
            .Returns(data.BuildMockDbSet().Object);

        var request = new GetUsersExpiredContractsDatesRequest.Query(targetDate);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        var dto = result.Single();
        dto.UserId.Should().Be(1);
        dto.ContractStartDate.Should().Be(new DateOnly(2023, 1, 1));
        dto.ContractEndDate.Should().Be(targetDate);
        dto.HireDate.Should().Be(new DateOnly(2023, 1, 1));
        dto.FireDate.Should().BeNull();
    }

    [Fact]
    public async Task HandleFireDateOverridesContractToAndMatchesDateIncludesResult()
    {
        // Arrange
        var targetDate = new DateOnly(2024, 3, 15);

        var data = new List<UserContract>
        {
            new UserContract
            {
                UserId = 10,
                IsActive = true,
                ContractFrom = new DateOnly(2022,6,1),
                ContractTo = new DateOnly(2025,6,1),
                FireDate = targetDate,
                User = new TeamBoardUser
                {
                    HireDate = new DateOnly(2022,6,1),
                    UserId = 10,
                    FirstName = "test",
                    LastName = "test",
                    Email = "test",
                }
            },
            new UserContract
            {
                UserId = 11,
                IsActive = true,
                ContractFrom = new DateOnly(2022,6,1),
                ContractTo = new DateOnly(2025,6,1),
                FireDate = new DateOnly(2024,3,14),
                User = new TeamBoardUser
                {
                    HireDate = new DateOnly(2022,6,1),
                    UserId = 11,
                    FirstName = "test",
                    LastName = "test",
                    Email = "test",
                }
            }
        };

        _dbContextMock
            .Setup(c => c.UserContracts)
            .Returns(data.BuildMockDbSet().Object);

        var request = new GetUsersExpiredContractsDatesRequest.Query(targetDate);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        var dto = result.Single();
        dto.UserId.Should().Be(10);
        dto.ContractEndDate.Should().Be(new DateOnly(2025, 6, 1));
        dto.FireDate.Should().Be(targetDate);
    }

    [Fact]
    public async Task HandleContractsWithNullContractEndIgnored()
    {
        // Arrange
        var data = new List<UserContract>
        {
            new UserContract
            {
                UserId = 5,
                IsActive = true,
                ContractFrom = new DateOnly(2023,1,1),
                ContractTo = null,
                FireDate = null,
                User = new TeamBoardUser
                {
                    HireDate = new DateOnly(2023,1,1),
                    UserId = 1,
                    FirstName = "test",
                    LastName = "test",
                    Email = "test",
                }
            }
        };

        _dbContextMock
            .Setup(c => c.UserContracts)
            .Returns(data.BuildMockDbSet().Object);

        var request = new GetUsersExpiredContractsDatesRequest.Query(new DateOnly(2024, 1, 1));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
