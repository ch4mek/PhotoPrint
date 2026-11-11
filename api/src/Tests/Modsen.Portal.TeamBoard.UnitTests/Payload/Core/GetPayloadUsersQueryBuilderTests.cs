using System.Globalization;
using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Payload.Core;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Payload.Core
{
    public class GetPayloadUsersQueryBuilderTests
    {
        private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();
        private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
        private readonly GetPayloadUsersQueryBuilder _queryBuilder;
        private readonly Mock<IIdentityContract> _identityContractMock = new();
        private readonly CancellationToken _token;

        public GetPayloadUsersQueryBuilderTests()
        {
            _queryBuilder = new GetPayloadUsersQueryBuilder(
                _teamBoardDbContextMock.Object,
                _authenticatedUserContextMock.Object,
                _identityContractMock.Object);
        }

        [Fact]
        public async Task BuildUserIdsWhenUserIdIsInvalidShouldReturnEmptyArray()
        {
            // Arrange
            List<TeamBoardUser> users =
            [
                new() {
                    UserId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test",
                    Contracts = [new() { UserId = 1, ContractFrom = new DateOnly(2025, 01, 01) }] },
                new() {
                    UserId = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "test",
                    Contracts = [new() { UserId = 2, ContractFrom = new DateOnly(2025, 01, 01) }] },
                new() {
                    UserId = 3,
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "test",
                    Contracts = [new() { UserId = 3, ContractFrom = new DateOnly(2025, 01, 01) }] }
            ];

            _teamBoardDbContextMock
                .Setup(x => x.TeamBoardUsers)
                .Returns(users.BuildMockDbSet().Object);

            var filter = new PayloadFilterCoreDto { UserId = "Invalid Id" };
            _queryBuilder.WithFilter(filter);
            _queryBuilder.ForYear(2025);

            // Act
            var result = await _queryBuilder.BuildUserIds(_token);

            // Assert
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task BuildUserIdsWhenUsersAreInternsReturnEmptyArray()
        {
            // Arrange
            List<TeamBoardUser> users =
            [
                new() {
                    UserId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test",
                    Position = PortalRoleNames.Intern,
                    Contracts = [new() { UserId = 1, ContractFrom = new DateOnly(2025, 01, 01) }] },
                new() {
                    UserId = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "test",
                    Position = PortalRoleNames.Intern,
                    Contracts = [new() { UserId = 2, ContractFrom = new DateOnly(2025, 01, 01) }] },
                new() {
                    UserId = 3,
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "test",
                    Position = PortalRoleNames.Intern,
                    Contracts = [new() { UserId = 3, ContractFrom = new DateOnly(2025, 01, 01) }] }
            ];

            _teamBoardDbContextMock
                .Setup(x => x.TeamBoardUsers)
                .Returns(users.BuildMockDbSet().Object);
            
            _queryBuilder.ForYear(2025);

            // Act
            var result = await _queryBuilder.BuildUserIds(_token);

            // Assert
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task BuildUserIdsWhenUserIdIsValidShouldFilterByUserId()
        {
            // Arrange
            var userId = 1;

            List<TeamBoardUser> users =
            [
                new() {
                    UserId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test",
                    Contracts = [new() { UserId = 1, ContractFrom = new DateOnly(2025, 01, 01) }] },
                new() {
                    UserId = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "test",
                    Contracts = [new() { UserId = 2, ContractFrom = new DateOnly(2025, 01, 01) }] },
                new() {
                    UserId = 3,
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "test",
                    Contracts = [new() { UserId = 3, ContractFrom = new DateOnly(2025, 01, 01) }] }
            ];

            _teamBoardDbContextMock
                .Setup(x => x.TeamBoardUsers)
                .Returns(users.BuildMockDbSet().Object);

            var filter = new PayloadFilterCoreDto {
                UserId = userId.ToString(CultureInfo.InvariantCulture) };
            _queryBuilder.WithFilter(filter);
            _queryBuilder.ForYear(2025);

            // Act
            var result = await _queryBuilder.BuildUserIds(_token);

            // Assert
            result.Should().HaveCount(1);
            result.First().Should().Be(userId);
        }

        [Fact]
        public async Task BuildUserIdsWhenOrderingByNameShouldOrderByUserFullName()
        {
            // Arrange
            List<TeamBoardUser> users =
            [
                new() {
                    UserId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test",
                    Contracts = [new() { UserId = 1, ContractFrom = new DateOnly(2025, 01, 01) }] },
                new() {
                    UserId = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "test",
                    Contracts = [new() { UserId = 2, ContractFrom = new DateOnly(2025, 01, 01) }] },
                new() {
                    UserId = 3,
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "test",
                    Contracts = [new() { UserId = 3, ContractFrom = new DateOnly(2025, 01, 01) }] }
            ];

            _teamBoardDbContextMock
                .Setup(x => x.TeamBoardUsers)
                .Returns(users.BuildMockDbSet().Object);
            var ordering = new PayloadOrderingCoreDto
            {
                Field = PayloadOrderingField.EmployeeFullName
            };

            _queryBuilder.WithOrdering(ordering);
            _queryBuilder.ForYear(2025);

            // Act
            var result = await _queryBuilder.BuildUserIds(_token);

            // Assert
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo([3, 1, 2]);
        }

        [Fact]
        public async Task BuildUserIdsWhenOrderingByIdShouldOrderByUserId()
        {
            // Arrange
            List<TeamBoardUser> users =
            [
                new() {
                    UserId = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "test",
                    Contracts = [new() { UserId = 2, ContractFrom = new DateOnly(2025, 01, 01) }] },
                new() {
                    UserId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test",
                    Contracts = [new() { UserId = 1, ContractFrom = new DateOnly(2025, 01, 01) }] },
                new() {
                    UserId = 3,
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "test",
                    Contracts = [new() { UserId = 3, ContractFrom = new DateOnly(2025, 01, 01) }] }
            ];

            _teamBoardDbContextMock
                .Setup(x => x.TeamBoardUsers)
                .Returns(users.BuildMockDbSet().Object);
            var ordering = new PayloadOrderingCoreDto
            {
                Field = PayloadOrderingField.UserId
            };

            _queryBuilder.WithOrdering(ordering);
            _queryBuilder.ForYear(2025);

            // Act
            var result = await _queryBuilder.BuildUserIds(_token);

            // Assert
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo([1, 2, 3]);
        }

        [Fact]
        public async Task BuildUserIdsWhenFilterBySpecializationNameIdShouldFilterBySpecializationName()
        {
            // Arrange
            var traineeName = "Trainee =";
            var middleName = "Middle =";

            List<TeamBoardUser> users =
            [
                new() {
                    UserId = 1,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "test",
                    Contracts = [new() { UserId = 2, ContractFrom = new DateOnly(2025, 01, 01) }],
                    UserSpecializations = [
                        new() { SpecializationTemplate = new SpecializationTemplate() { Name = middleName} } ] },
                new() {
                    UserId = 2,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test",
                    Contracts = [new() { UserId = 1, ContractFrom = new DateOnly(2025, 01, 01) }],
                    UserSpecializations = [
                        new() { SpecializationTemplate = new SpecializationTemplate() { Name = traineeName} } ] },
                new() {
                    UserId = 3,
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "test",
                    Contracts = [new() { UserId = 3, ContractFrom = new DateOnly(2025, 01, 01) }],
                    UserSpecializations = [
                        new() { SpecializationTemplate = new SpecializationTemplate() { Name = middleName} } ] },
            ];

            _teamBoardDbContextMock
                .Setup(x => x.TeamBoardUsers)
                .Returns(users.BuildMockDbSet().Object);

            var filter = new PayloadFilterCoreDto
            {
                SpecializationName = middleName
            };
            _queryBuilder.WithFilter(filter);
            _queryBuilder.ForYear(2025);

            // Act
            var result = await _queryBuilder.BuildUserIds(_token);

            // Assert
            result.Should().BeEquivalentTo([1, 3]);
        }

        [Fact]
        public async Task BuildUserIdsWhenFilterByDepartmentShouldFilterByDepartment()
        {
            // Arrange
            var netDepartmentId = "NET";
            var javaDepartmentId = "Java";

            List<TeamBoardUser> users =
            [
                new() {
                    UserId = 1,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "test",
                    Contracts = [new() { UserId = 2, ContractFrom = new DateOnly(2025, 01, 01) }],
                    DepartmentId = netDepartmentId },
                new() {
                    UserId = 2,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test",
                    Contracts = [new() { UserId = 1, ContractFrom = new DateOnly(2025, 01, 01) }],
                    DepartmentId = javaDepartmentId },
                new() {
                    UserId = 3,
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "test",
                    Contracts = [new() { UserId = 3, ContractFrom = new DateOnly(2025, 01, 01) }],
                    DepartmentId = javaDepartmentId },
            ];

            _identityContractMock
                .Setup(x => x.UsersWorkInDepartment(2025, It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>(), null))
                .ReturnsAsync([1]);

            _teamBoardDbContextMock
                .Setup(x => x.TeamBoardUsers)
                .Returns(users.BuildMockDbSet().Object);

            var filter = new PayloadFilterCoreDto
            {
                DepartmentIds = [netDepartmentId]
            };
            _queryBuilder.WithFilter(filter);
            _queryBuilder.ForYear(2025);

            // Act
            var result = await _queryBuilder.BuildUserIds(_token);

            // Assert
            result.Should().BeEquivalentTo([1]);
        }

        [Fact]
        public async Task BuildUserIdsWhenUserIsHeadOfDepartmentShouldFilterByDepartment()
        {
            // Arrange
            var netDepartmentId = "NET";
            var javaDepartmentId = "Java";

            List<TeamBoardUser> users =
            [
                new() {
                    UserId = 1,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "test",
                    Contracts = [new() { UserId = 2, ContractFrom = new DateOnly(2025, 01, 01) }],
                    DepartmentId = netDepartmentId },
                new() {
                    UserId = 2,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test",
                    Contracts = [new() { UserId = 1, ContractFrom = new DateOnly(2025, 01, 01) }],
                    DepartmentId = javaDepartmentId },
                new() {
                    UserId = 3,
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "test",
                    Contracts = [new() { UserId = 3, ContractFrom = new DateOnly(2025, 01, 01) }],
                    DepartmentId = javaDepartmentId },
            ];

            _identityContractMock
                .Setup(x => x.UsersWorkInDepartment(2025, It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>(), null))
                .ReturnsAsync([1]);

            _teamBoardDbContextMock
                .Setup(x => x.TeamBoardUsers)
                .Returns(users.BuildMockDbSet().Object);

            _authenticatedUserContextMock
                .Setup(x => x.GetRoles())
                .Returns([PortalRoleNames.HeadOfDepartment]);
            _authenticatedUserContextMock
                .Setup(x => x.DepartmentId)
                .Returns(netDepartmentId);

            _queryBuilder.ForYear(2025);

            // Act
            var result = await _queryBuilder.BuildUserIds(_token);

            // Assert
            result.Should().BeEquivalentTo([1]);
        }

        [Fact]
        public async Task BuildUserIdsWhenUserIsGroupManagerShouldFilterByGroup()
        {
            // Arrange
            var groupId1 = 1;
            var groupId2 = 2;

            List<TeamBoardUser> users =
            [
                new() {
                    UserId = 1,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "test",
                    Contracts = [new() { UserId = 2, ContractFrom = new DateOnly(2025, 01, 01) }],
                    ResourceGroupId = groupId1 },
                new() {
                    UserId = 2,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test",
                    Contracts = [new() { UserId = 1, ContractFrom = new DateOnly(2025, 01, 01) }],
                    ResourceGroupId = groupId2 },
                new() {
                    UserId = 3,
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "test",
                    Contracts = [new() { UserId = 3, ContractFrom = new DateOnly(2025, 01, 01) }],
                    ResourceGroupId = groupId1 },
            ];

            _teamBoardDbContextMock
                .Setup(x => x.TeamBoardUsers)
                .Returns(users.BuildMockDbSet().Object);

            _authenticatedUserContextMock
                .Setup(x => x.GetRoles())
                .Returns([PortalRoleNames.GroupManager]);
            _authenticatedUserContextMock
                .Setup(x => x.ResourceGroupId)
                .Returns(groupId1);

            _queryBuilder.ForYear(2025);

            // Act
            var result = await _queryBuilder.BuildUserIds(_token);

            // Assert
            result.Should().BeEquivalentTo([1, 3]);
        }

        [Fact]
        public async Task BuildUserIdsWhenFilterByNeedProjectShouldFilterByNeedProject()
        {
            // Arrange
            List<TeamBoardUser> users =
            [
                new() {
                    UserId = 1,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "test",
                    Contracts = [new() { UserId = 2, ContractFrom = new DateOnly(2025, 01, 01) }],
                    UserPayloadDetails = [new UserPayloadDetails() { NeedProject = true }] },
                new() {
                    UserId = 2,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test",
                    Contracts = [new() { UserId = 1, ContractFrom = new DateOnly(2025, 01, 01) }],
                    UserPayloadDetails = [new UserPayloadDetails() { NeedProject = false }] },
                new() {
                    UserId = 3,
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "test",
                    Contracts = [new() { UserId = 3, ContractFrom = new DateOnly(2025, 01, 01) }] },
            ];

            _teamBoardDbContextMock
                .Setup(x => x.TeamBoardUsers)
                .Returns(users.BuildMockDbSet().Object);

            var filter = new PayloadFilterCoreDto
                { NeedProject = true };
            _queryBuilder.WithFilter(filter);
            _queryBuilder.ForYear(2025);

            // Act
            var result = await _queryBuilder.BuildUserIds(_token);

            // Assert
            result.Should().BeEquivalentTo([1]);
        }
    }
}
